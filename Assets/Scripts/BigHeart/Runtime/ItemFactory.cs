using System;
using CMD.Base;
using CMD.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BigHeart
{
    public sealed class ItemFactory : IEntityFactory<ItemRuntime, ItemDefinition>
    {
        public ItemRuntime Create(ItemDefinition definition, Vector3 position, Quaternion rotation)
        {
            if (definition is not IPrefabProvider pp || pp.Prefab == null)
                throw new InvalidOperationException($"Definition '{definition?.name}' must provide a Prefab.");

            var go = Object.Instantiate(pp.Prefab, position, rotation);
            var runtime = go.GetComponent<ItemRuntime>()
                ?? throw new InvalidOperationException($"Prefab '{pp.Prefab.name}' must have BaseEntityRuntime.");
            runtime.Init(definition);
            return runtime;
        }

        public ItemRuntime CreateFromSave(EntitySaveData saveData, ICatalog<ItemDefinition> catalog)
        {
            var def = catalog.GetByKey(saveData.definitionKey);
            if (def is not IPrefabProvider pp || pp.Prefab == null)
                throw new InvalidOperationException($"Definition '{def?.name}' must provide a Prefab.");

            var go = Object.Instantiate(pp.Prefab);
            if (System.Guid.TryParse(saveData.entityId, out var guid))
                go.GetComponent<StableId>()?.SetFromSave(guid);

            var runtime = go.GetComponent<ItemRuntime>()
                ?? throw new InvalidOperationException($"Prefab '{pp.Prefab.name}' must have BaseEntityRuntime.");

            runtime.Init(def);
            runtime.Restore(saveData);
            return runtime;
        }
    }
}
