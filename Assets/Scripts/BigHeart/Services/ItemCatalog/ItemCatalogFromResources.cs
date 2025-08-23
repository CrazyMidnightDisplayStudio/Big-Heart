using System.Collections.Generic;
using System.Linq;
using CMD.Core;
using UnityEngine;

namespace BigHeart.Services
{
    public class ItemCatalogFromResources : ICatalog<ItemDefinition>
    {
        private readonly List<string> _roots;
        private readonly Dictionary<string, ItemDefinition> _itemsByKey;
        private bool _loaded;

        public ItemCatalogFromResources(params string[] roots)
        {
            _roots = (roots != null && roots.Length > 0) ? new List<string>(roots) : new List<string>();
            _itemsByKey = new Dictionary<string, ItemDefinition>();
            _loaded = false;
        }

        public ItemDefinition GetByKey(string key)
        {
            EnsureLoaded();
            if (_itemsByKey.TryGetValue(key, out var def)) return def;
            throw new System.Collections.Generic.KeyNotFoundException($"Definition key '{key}' not found in Resources catalog.");
        }

        public bool TryGetByKey(string key, out ItemDefinition def)
        {
            EnsureLoaded();
            return _itemsByKey.TryGetValue(key, out def);
        }

        public void EnsureLoaded()
        {
            if (_loaded) return;
            _loaded = true;

            foreach (var root in _roots)
            {
                // Загружаем все ScriptableObject’ы с базовым типом
                var definitions = Resources.LoadAll<ItemDefinition>(root);
                foreach (var d in definitions.Where(d => d))
                {
                    var key = d.Key;
                    if (string.IsNullOrEmpty(key))
                    {
                        Debug.LogError($"Definition '{d.name}' has empty Key", d);
                        continue;
                    }
                    if (_itemsByKey.ContainsKey(key))
                    {
                        Debug.LogError($"Duplicate definition key '{key}'. First: '{_itemsByKey[key].name}', second: '{d.name}'", d);
                        continue;
                    }
                    _itemsByKey[key] = d;
                }

            }
        }
    }
}
