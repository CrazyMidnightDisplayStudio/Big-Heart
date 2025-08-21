using System;
using CMD.Entities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BigHeart
{
    public sealed class PrefabEntityFactory : IEntityFactory
    {
        public BaseEntityRuntime Create(EntityDefinitionSO definition, Vector3 pos, Quaternion rot)
        {
            if (definition is not IPrefabProvider pp || pp.Prefab == null)
                throw new InvalidOperationException($"Definition '{definition?.name}' must provide a Prefab.");

            var go = Object.Instantiate(pp.Prefab, pos, rot);
            var runtime = go.GetComponent<BaseEntityRuntime>()
                ?? throw new InvalidOperationException($"Prefab '{pp.Prefab.name}' must have BaseEntityRuntime.");
            runtime.Init(definition);
            return runtime;
        }

        public BaseEntityRuntime CreateFromSave(EntitySaveData save, IEntityCatalog catalog)
        {
            var def = catalog.GetByKey(save.definitionKey);
            if (def is not IPrefabProvider pp || pp.Prefab == null)
                throw new InvalidOperationException($"Definition '{def?.name}' must provide a Prefab.");

            var go = Object.Instantiate(pp.Prefab);
            if (System.Guid.TryParse(save.entityId, out var guid))
                go.GetComponent<StableId>()?.SetFromSave(guid);

            var runtime = go.GetComponent<BaseEntityRuntime>()
                ?? throw new InvalidOperationException($"Prefab '{pp.Prefab.name}' must have BaseEntityRuntime.");

            runtime.Init(def);
            runtime.Restore(save);
            return runtime;
        }
    }
}
