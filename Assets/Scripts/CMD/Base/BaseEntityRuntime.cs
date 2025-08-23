using CMD.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CMD.Base
{
    /// <summary>
    /// Тонкая база: состояние, ссылка на definition, сейв/лоад.
    /// НИКАКОГО UI/префабов/иконок здесь нет.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(StableId))]
    public abstract class BaseEntityRuntime : MonoBehaviour
    {
        [SerializeField] private EntityRuntimeState state = new(); // runtime-состояние
        [SerializeField] private EntityDefinitionSO definition; // описание/правила
        [SerializeField] private bool destroyOnRemoveFromGame = true;

        public EntityDefinitionSO Definition => definition;
        public EntityRuntimeState State => state;
        public string EntityId => _stableId.IdString;
        public bool IsRetired { get; private set; }

        private StableId _stableId;

        protected virtual void Awake()
        {
            _stableId = GetComponent<StableId>() ?? gameObject.AddComponent<StableId>();
        }

        /// <summary>Инициализация из фабрики/бутстрапа.</summary>
        public virtual void Init(EntityDefinitionSO def)
        {
            definition = def;
        }

        public virtual EntitySaveData CaptureData()
        {
            return new EntitySaveData
            {
                entityId = EntityId,
                definitionKey = definition ? definition.Key : string.Empty,
                retired = IsRetired,
                stateJson = state != null ? JsonUtility.ToJson(state) : string.Empty,
                location = new EntityLocation
                {
                    kind = EEntityLocationKind.world,
                    scene = SceneManager.GetActiveScene().name,
                    position = transform.position,
                    rotation = transform.rotation
                }
            };
        }

        public virtual void Restore(EntitySaveData data)
        {
            if (data == null) return;

            IsRetired = data.retired;

            if (!string.IsNullOrEmpty(data.stateJson) && state != null)
            {
                try { JsonUtility.FromJsonOverwrite(data.stateJson, state); } catch { /* ignore */ }
            }

            if (data.location != null && data.location.kind == EEntityLocationKind.world)
                transform.SetPositionAndRotation(data.location.position, data.location.rotation);

            gameObject.SetActive(!IsRetired);
        }

        /// <summary>Зафиксировать состояние в репозиторий через контекст.</summary>
        public void Capture()
        {
            var saves = ServiceRegistry.Get<ISaveRepository>();
            saves.Upsert(CaptureData());
        }

        /// <summary>Убрать из игры (возможное уничтожение объекта).</summary>
        public virtual void RemoveFromGame()
        {
            IsRetired = true;
            Capture();
            if (destroyOnRemoveFromGame) Destroy(gameObject);
            else gameObject.SetActive(false);
        }
    }
}
