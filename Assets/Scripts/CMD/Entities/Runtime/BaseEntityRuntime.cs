using System.Collections.Generic;
using CMD.Core;
using UnityEngine;

namespace CMD.Entities
{
    /// <summary>
    /// Базовый класс геймплейной сущьности, теоретически может быть чем угодно.
    /// Создается из описания предмета - definition
    /// Геймплейная логика создается отдельно и хранится списком GameplayRuleSO внутри definition
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(StableId))]
    public abstract class BaseEntityRuntime : MonoBehaviour
    {
        [SerializeField] private EntityRuntimeState state = new(); // Состояние объекта (может быть не изменяемым, это нормально)
        [SerializeField] private EntityDefinitionSO definition; // ScriptableObject - описание сущьности, так же содержит геймплейную логику GameplayRuleSO
        [SerializeField] private bool destroyOnRemoveFromGame = true; // нужно ли уничтожать MB при вызове RemoveFromGame

        public EntityRuntimeState State => state;
        public EntityDefinitionSO Definition => definition;
        public string EntityId => _stableId.IdString;
        public bool IsRetired { get; private set; }

        private StableId _stableId;
        private IGameContext _ctx;
        private readonly List<ITriggerRuntime> _installed = new();

        private bool _initialized = false;
        private bool _installedReactions = false;

        private bool ReadyToRun => _initialized && isActiveAndEnabled;

        /// <summary>
        /// Инициализация - вызови сразу после спавна из фабрики!
        /// </summary>
        /// <param name="def">Описание сущьности</param>
        /// <param name="ctx">Контекст игры содержит нужные зависимости</param>
        /// <param name="preload">Состояние объекта из сейва</param>
        public void Init(EntityDefinitionSO def, IGameContext ctx, EntitySaveData preload = null)
        {
            definition = def;
            _ctx = ctx;
            _initialized = true;

            if (preload != null)
            {
                if (System.Guid.TryParse(preload.entityId, out var g))
                {
                    _stableId.SetFromSave(g);
                }
                Restore(preload); // позиция/rotation/consumed
            }

            if (isActiveAndEnabled)
            {
                InstallGameplayRules();
            }
        }

        private void Awake()
        {
            _stableId = GetComponent<StableId>();
            _ctx ??= GameContextLocator.Current;
        }

        private void Start()
        {
            if (!_initialized)
            {
                Debug.LogError($"[{name}] EntityRuntime has not been initialized via Init(...). " +
                    "Make sure you create it using the factory or call Init() yourself.");
            }
        }

        private void OnEnable()
        {
            if (ReadyToRun)
            {
                InstallGameplayRules();
            }
        }
        private void OnDisable() => UninstallGameplayRules();

        protected virtual void InstallGameplayRules()
        {
            if (_installedReactions || definition == null || IsRetired) return;
            _installedReactions = true;

            foreach (var rule in definition.gameplayRules)
            {
                if (rule?.trigger == null || rule.effects == null || rule.effects.Count == 0)
                {
                    continue;
                }

                var trig = rule.trigger.CreateRuntime(this, _ctx);
                var localEffects = new List<IEffectRuntime>(rule.effects.Count);
                foreach (var fx in rule.effects)
                {
                    localEffects.Add(fx.CreateRuntime(this, _ctx));
                }

                trig.Fired += () =>
                {
                    foreach (var e in localEffects)
                    {
                        e.Execute();
                    }
                };

                trig.Install();
                _installed.Add(trig);
            }
        }

        protected virtual void UninstallGameplayRules()
        {
            foreach (var t in _installed) t.Uninstall();
            _installed.Clear();
            _installedReactions = false;
        }

        /// <summary>
        /// Выводит сущьность из игры
        /// </summary>
        public virtual void RemoveFromGame()
        {
            if (IsRetired) return;
            IsRetired = true;
            Capture(); // Фиксируем текущее состояние в памяти, но не выполняем Flush(само сохранение в файл)
            UninstallGameplayRules(); // Убирает геймплейные правила и эффекты
            if (destroyOnRemoveFromGame) Destroy(gameObject);
            else gameObject.SetActive(false);
        }

        /// <summary>
        /// Создаем данные для сохранения и копируем сериализуемые поля из рантайм объекта.
        /// Только для мировых сущьностей
        /// </summary>
        /// <returns>Объект сохранения</returns>
        public virtual EntitySaveData CaptureData() => new EntitySaveData
        {
            definitionKey = definition?.Key,
            entityId = EntityId,
            retired = IsRetired,
            location = new EntityLocation
            {
                kind = EEntityLocationKind.world,
                scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
                position = transform.position,
                rotation = transform.rotation,
            },
            stateJson = JsonUtility.ToJson(State)
        };

        public virtual void Restore(EntitySaveData entitySaveData)
        {
            if (entitySaveData == null || entitySaveData.entityId != EntityId)
            {
                return;
            }

            if (!IsRetired && entitySaveData.retired)
            {
                IsRetired = true; // на случай пост-факта
            }

            transform.position = entitySaveData.location.position;
            transform.rotation = entitySaveData.location.rotation;

            if (!string.IsNullOrEmpty(entitySaveData.stateJson))
                JsonUtility.FromJsonOverwrite(entitySaveData.stateJson, state);

            if (IsRetired)
            {
                if (destroyOnRemoveFromGame)
                {
                    Destroy(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Зафиксировать текущее состояние объекта в сейве in-memory
        /// </summary>
        public void Capture() => _ctx.Saves.Upsert(CaptureData());
    }
}
