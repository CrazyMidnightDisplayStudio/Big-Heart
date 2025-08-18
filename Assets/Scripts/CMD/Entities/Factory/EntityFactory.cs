using System;
using CMD.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CMD.Entities
{
    public sealed class EntityFactory : IEntityFactory
    {
        readonly IGameContext _ctx;
        public EntityFactory(IGameContext ctx) { _ctx = ctx; }

        public BaseEntityRuntime Create(EntityDefinitionSO definition, Vector3 position, Quaternion rotation)
        {
            var prefab = definition.viewPrefab ?? throw new InvalidOperationException(
                $"Entity '{definition.Key}' must have a viewPrefab with a component derived from BaseEntityRuntime.");

            var go = Object.Instantiate(prefab, position, rotation);
            var runtime = go.GetComponent<BaseEntityRuntime>();
            if (runtime == null)
            {
                throw new InvalidOperationException(
                    $"Prefab '{definition.Key}' must contain a component derived from BaseEntityRuntime.");
            }
            runtime.Init(definition, _ctx);
            return runtime;
        }

        public BaseEntityRuntime CreateFromSave(EntitySaveData saveData, IEntityCatalog catalog)
        {
            var definition = catalog.GetByKey(saveData.definitionKey);
            var go = Object.Instantiate(definition.viewPrefab, saveData.location.position, saveData.location.rotation);
            var runtime = go.GetComponent<BaseEntityRuntime>()
                ?? throw new InvalidOperationException($"Prefab '{definition.Key}' must have BaseEntityRuntime-derived component.");

            // ВАЖНО: id из сейва - ДО Init, чтобы EntityId совпал
            if (System.Guid.TryParse(saveData.entityId, out var guid))
            {
                go.GetComponent<StableId>()?.SetFromSave(guid);
            }

            runtime.Init(definition, _ctx, saveData); // preload -> внутри Restore до установки правил
            return runtime;
        }
    }
}
