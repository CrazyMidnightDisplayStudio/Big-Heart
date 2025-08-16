using System.Collections.Generic;
using UnityEngine;

namespace CMD.Core
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(StableId))]
    public sealed class EntityRuntime : MonoBehaviour
    {
        [SerializeField] private EntityDefinitionSO definition;
        [SerializeField] private bool destroyOnConsumed = true;

        public EntityDefinitionSO Definition => definition;
        public string InstanceId => _stableId.IdString;
        public bool Consumed { get; private set; }

        private StableId _stableId;
        private IGameContext _ctx;
        private readonly List<ITriggerRuntime> _installed = new();

        private bool _initialized = false;
        private bool _installedRules = false;
        private bool _readyToRun => _initialized && isActiveAndEnabled;

        // вызови сразу после инстанса от фабрики/спавнера
        public void Init(EntityDefinitionSO def, IGameContext ctx, EntitySaveData preload = null)
        {
            definition = def;
            _ctx = ctx;
            _initialized = true;

            if (preload != null)
            {
                if (System.Guid.TryParse(preload.instanceId, out var g))
                {
                    _stableId.SetFromSave(g);
                }
                Restore(preload); // позиция/rotation/consumed
            }

            if (isActiveAndEnabled)
            {
                InstallRules();
            }
        }

        private void Awake()
        {
            if (!_initialized)
            {
                Debug.LogError($"[{name}] EntityRuntime has not been initialized via Init(...). " +
                    "Make sure you create it using the factory or call Init() yourself.");
            }

            _stableId = GetComponent<StableId>();
            _ctx ??= GameContextLocator.Current;
        }

        private void OnEnable()
        {
            if (_readyToRun)
            {
                InstallRules();
            }
        }
        private void OnDisable() => UninstallRules();
        private void OnDestroy() => Persist();

        void InstallRules()
        {
            if (_installedRules || definition == null || Consumed) return;
            _installedRules = true;

            foreach (var rule in definition.rules)
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
                    Persist();
                };

                trig.Install();
                _installed.Add(trig);
            }
        }

        private void UninstallRules()
        {
            foreach (var t in _installed) t.Uninstall();
            _installed.Clear();
            _installedRules = false;
        }

        public void Consume()
        {
            if (Consumed) return;
            Consumed = true;
            Persist();
            UninstallRules();
            if (destroyOnConsumed) Destroy(gameObject);
            else gameObject.SetActive(false);
        }

        public EntitySaveData Capture() => new EntitySaveData
        {
            definitionKey = definition?.Key,
            instanceId = InstanceId,
            position = transform.position,
            rotation = transform.rotation,
            consumed = Consumed
        };

        public void Restore(EntitySaveData entitySaveData)
        {
            if (entitySaveData == null || entitySaveData.instanceId != InstanceId)
            {
                return;
            }

            if (!Consumed && entitySaveData.consumed)
            {
                Consumed = true; // на случай пост-факта
            }

            transform.position = entitySaveData.position;
            transform.rotation = entitySaveData.rotation;

            if (Consumed)
            {
                if (destroyOnConsumed)
                {
                    Destroy(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public void Persist() => _ctx.Saves.Upsert(Capture());
    }
}
