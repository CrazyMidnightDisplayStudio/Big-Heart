using UnityEngine;
using Base;

namespace ItemSystem
{
    public sealed class ItemFactory : IEntityFactory<ItemPresenter, ItemDefinition>
    {
        private readonly GameObject _genericPrefab;
        private readonly EntityRegistry _registry = EntityRegistry.Instance;

        public ItemFactory(GameObject genericPrefab) => _genericPrefab = genericPrefab;

        public ItemPresenter Spawn(ItemDefinition def, Vector3 pos, Transform parent = null)
        {
            if (def == null)
            {
                throw new System.ArgumentNullException(nameof(def));
            }

            GameObject prefab = def.overridePrefab ?? _genericPrefab;
            GameObject go = prefab != null
                ? Object.Instantiate(prefab, pos, Quaternion.identity, parent)
                : new GameObject($"Item_{(string.IsNullOrWhiteSpace(def.displayName) ? def.name : def.displayName)}", typeof(SpriteRenderer));

            var presenter = go.GetComponent<ItemPresenter>() ?? go.AddComponent<ItemPresenter>();
            presenter.Init(def);
            return presenter;
        }
    }
}
