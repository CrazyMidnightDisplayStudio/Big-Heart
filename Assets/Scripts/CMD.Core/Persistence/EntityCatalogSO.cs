using System.Collections.Generic;
using UnityEngine;

namespace CMD.Core
{
    /// <summary>
    /// Ручной список предметов в инспекторе
    /// </summary>
    [CreateAssetMenu(menuName = "Core.Items/Item Catalog", fileName = "ItemCatalog")]
    public sealed class ItemCatalogSO : ScriptableObject, IEntityCatalog
    {
        [SerializeField] private List<EntityDefinitionSO> entities = new();

        Dictionary<string, EntityDefinitionSO> _map;

        void OnEnable()
        {
            _map = new Dictionary<string, EntityDefinitionSO>();
            foreach (var definition in entities)
            {
                if (definition == null || string.IsNullOrEmpty(definition.Key)) continue;
                if (_map.ContainsKey(definition.Key))
                {
                    Debug.LogWarning($"Duplicate item key: {definition.Key}", definition);
                }
                _map[definition.Key] = definition;
            }
        }

        public EntityDefinitionSO GetByKey(string key)
        {
            if (!_map.TryGetValue(key, out var def))
                throw new System.Collections.Generic.KeyNotFoundException($"Item '{key}' not found.");
            return def;
        }

        public bool TryGetByKey(string key, out EntityDefinitionSO def) => _map.TryGetValue(key, out def);
    }
}
