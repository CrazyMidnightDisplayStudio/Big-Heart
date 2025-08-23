using System;
using System.Collections.Generic;
using System.Linq;
using BigHeart.Services;
using CMD.Base;
using CMD.Core;
using CMD.Events.ContainmentEvents;
using CMD.SaveLoadSystem;
using CMD.Services;
using UnityEngine;
using Object = UnityEngine.Object;
namespace BigHeart.Save
{
    /// <summary>
    /// Оркестратор сейвов/лоадов:
    ///  - Save: собирает всех ISaveLoadObject в сцене и отдаёт их стратегии.
    ///  - Load: чистит сцену, создаёт runtime-сущности из чанков, раскладывает их по контейнерам,
    ///          затем раздаёт остальные чанки компонентам (UI/настройки и т.п.).
    /// </summary>
    public sealed class SaveLoadService : ISaveLoadService
    {
        private readonly ISaveLoadStrategy _strategy;

        // Зависимости игры (типизировано под Item; легко расширить позже).
        private readonly ICatalog<ItemDefinition> _itemCatalog;
        private readonly IEntityFactory<ItemRuntime, ItemDefinition> _itemFactory;
        private readonly IContainmentService _containment;

        public SaveLoadService(ISaveLoadStrategy strategy)
        {
            _strategy     = strategy ?? throw new ArgumentNullException(nameof(strategy));
            _itemCatalog  = ServiceRegistry.Get<ICatalog<ItemDefinition>>();
            _itemFactory  = ServiceRegistry.Get<IEntityFactory<ItemRuntime, ItemDefinition>>();
            _containment  = ServiceRegistry.Get<IContainmentService>();
        }

        /*──────────────────────── Save ────────────────────────*/
        public void SaveGame()
        {
            var participants = Object
                .FindObjectsOfType<MonoBehaviour>(includeInactive: true)
                .OfType<ISaveLoadObject>()
                .ToArray();

            _strategy.Save(participants);
            Debug.Log($"[Save] Collected {participants.Length} objects and wrote via {_strategy.GetType().Name}");
        }

        /*──────────────────────── Load ────────────────────────*/
        public void LoadGameFresh()
        {
            var chunks = _strategy.Load() ?? Array.Empty<SaveLoadData>();

            // 0) Удаляем все текущие runtime-сущности (инвентари/контейнеры не трогаем)
            foreach (var e in Object.FindObjectsOfType<BaseEntityRuntime>(includeInactive: true))
                Object.Destroy(e.gameObject);

            // 1) Делим чанки: сущности vs остальные
            var entityChunks    = chunks.Where(IsEntityChunk).ToList();
            var nonEntityChunks = chunks.Except(entityChunks).ToList();

            var created = new Dictionary<string, BaseEntityRuntime>(entityChunks.Count);

            // 2) Создаём все сущности. События контейнеров подавим до момента раскладки.
            using (_containment.SuppressEventsScope())
            {
                foreach (var c in entityChunks)
                {
                    EntitySaveData dto;
                    try { dto = c.Read<EntitySaveData>(); }
                    catch (Exception ex) { Debug.LogException(ex); continue; }

                    if (string.IsNullOrEmpty(dto.definitionKey))
                    {
                        Debug.LogWarning($"[Load] Empty definitionKey for chunk {c.Id}");
                        continue;
                    }

                    // Пока поддерживаем только ItemDefinition/ItemRuntime.
                    if (_itemCatalog.TryGetByKey(dto.definitionKey, out var itemDef))
                    {
                        var rt = _itemFactory.Create(itemDef, Vector3.zero, Quaternion.identity);

                        // Важно: восстановить стабильный GUID ДО Restore, если он есть в сейве.
                        if (Guid.TryParse(dto.entityId, out var gid))
                            rt.GetComponent<StableId>()?.SetFromSave(gid);

                        rt.Restore(dto);
                        created[rt.EntityId] = rt;
                    }
                    else
                    {
                        Debug.LogWarning($"[Load] Unknown definition '{dto.definitionKey}' — no catalog/factory registered for it. Chunk {c.Id} skipped.");
                    }
                }

                // 3) Разложим по контейнерам на основе location у каждого dto.
                foreach (var c in entityChunks)
                {
                    EntitySaveData dto;
                    try { dto = c.Read<EntitySaveData>(); }
                    catch { continue; }

                    if (dto.location == null) continue;

                    if (dto.location.kind == EEntityLocationKind.container &&
                        created.TryGetValue(dto.entityId, out var ent))
                    {
                        // ownerId + containerKey + index (+ slotKey, если у тебя есть)
                        _containment.Put(
                            dto.location.ownerId,
                            dto.location.containerKey,
                            ent,
                            dto.location.index,
                            dto.location.slotKey,
                            EChangeOrigin.load   // пометим источник
                        );
                    }
                    else if (dto.location.kind == EEntityLocationKind.world &&
                             created.TryGetValue(dto.entityId, out var entWorld))
                    {
                        // позиция уже восстановлена в BaseEntityRuntime.Restore(dto)
                        // ничего делать не нужно
                    }
                }
            }

            // 4) Раздаём остальные чанки адресно соответствующим компонентам по их SaveId.
            var recipients = Object.FindObjectsOfType<MonoBehaviour>(includeInactive: true)
                               .OfType<ISaveLoadObject>()
                               .ToDictionary(o => o.ComponentSaveId, o => o);

            int applied = 0;
            foreach (var c in nonEntityChunks)
            {
                if (recipients.TryGetValue(c.Id, out var target))
                {
                    try { target.RestoreData(c); applied++; }
                    catch (Exception ex) { Debug.LogException(ex); }
                }
            }

            Debug.Log($"[Load] Created entities={created.Count}, applied component chunks={applied}");
        }

        /*──────────────────────── Helpers ────────────────────────*/
        private static bool IsEntityChunk(SaveLoadData c)
        {
            // Совместимо и с флагом ESaveType.Entity, и с Id-форматом "entity:{guid}"
            return c.Type == ESaveType.entity || (c.Id != null && c.Id.StartsWith("entity:", StringComparison.Ordinal));
        }
    }
}
