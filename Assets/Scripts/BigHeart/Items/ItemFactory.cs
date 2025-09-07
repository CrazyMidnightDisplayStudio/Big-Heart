using System;
using CMD.Base;
using CMD.Common;
using CMD.SaveLoadSystem;
using CMD.Services;
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
            if (go.TryGetComponent<ItemView>(out var view))
            {
                view.Bind(runtime);
            }
            else
            {
                Debug.LogError($"Prefab '{pp.Prefab.name}' must have BaseEntityView.");
            }
            return runtime;
        }

        public ItemRuntime CreateFromSave(SaveLoadData entityChunk, ICatalog catalog)
        {
            if (entityChunk.Type != ESaveType.entity)
                throw new InvalidOperationException($"Expected entity chunk, got {entityChunk.Type}");

            // 1) DTO из JSON
            var dto = entityChunk.Read<EntitySaveData>();

            // 2) Definition + Prefab
            var def = catalog.Get<BigHeart.ItemDefinition>(dto.definitionKey)
                ?? throw new InvalidOperationException($"Definition '{dto.definitionKey}' not found");

            if (def is not IPrefabProvider pp || pp.Prefab == null)
                throw new InvalidOperationException($"Definition '{def.name}' must provide a Prefab");

            // 3) Инстанс
            var go = Object.Instantiate(pp.Prefab);

            // 4) Восстановить StableId заранее
            var stable = go.GetComponent<StableId>() ?? go.AddComponent<StableId>();
            if (!TryParseGuidLoose(dto.entityId, out var guid))
                throw new InvalidOperationException($"Bad entityId '{dto.entityId}'");
            stable.SetFromSave(guid);

            // 5) Рантайм
            var runtime = go.GetComponent<ItemRuntime>()
                ?? throw new InvalidOperationException($"Prefab '{pp.Prefab.name}' must have ItemRuntime");

            runtime.Init(def);
            if (go.TryGetComponent<ItemView>(out var view))
            {
                view.Bind(runtime);
            }
            else
            {
                Debug.LogError($"Prefab '{pp.Prefab.name}' must have BaseEntityView.");
            }

            // 6) Применить стейт и (если нужно) позу мира.
            //    Локацию 'Container' применять НЕ здесь — это фаза раскладки после спавна.
            runtime.RestoreFromDto(dto, initDefinition: false);

            return runtime;

            static bool TryParseGuidLoose(string s, out System.Guid g)
            {
                // допускаем "D" и "N" формы
                if (System.Guid.TryParse(s, out g)) return true;
                if (!string.IsNullOrEmpty(s) && s.Length == 32)
                    return System.Guid.TryParseExact(s, "N", out g);
                return false;
            }
        }
    }
}
