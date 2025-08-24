using CMD.Common;
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
        [SerializeField] private EntityDefinition definition; // описание/правила

        public EntityDefinition Definition => definition;
        public string EntityId => _stableId.IdString;

        private StableId _stableId;

        protected virtual void Awake()
        {
            _stableId = GetComponent<StableId>() ?? gameObject.AddComponent<StableId>();
        }

        /// <summary>Инициализация из фабрики/бутстрапа.</summary>
        public virtual void Init(EntityDefinition def)
        {
            definition = def;
        }
    }
}
