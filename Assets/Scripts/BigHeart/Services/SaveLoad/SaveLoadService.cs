using System;
using System.Collections.Generic;
using System.Linq;
using CMD.Base;
using CMD.Common;
using CMD.Core;
using CMD.Events.ContainmentEvents;
using CMD.SaveLoadSystem;
using CMD.Services;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BigHeart.Services
{
    /// Маркер, чтобы НЕ удалять объект при LoadGameFresh (например, игрок/его контейнеры и UI-якоря).
    public interface IPersistentRuntime
    {
    }

    /// Политика размещения при загрузке (на случай занятых слотов и т.п.)
    public interface IPlacementPolicy
    {
        bool TryPlace(IContainmentService cs, BaseEntityRuntime entity, ContainerData cd);
    }

    /// Строгая политика: строго в указанный слот; если занято — лог и возврат false.
    public sealed class StrictPlacementPolicy : IPlacementPolicy
    {
        public bool TryPlace(IContainmentService cs, BaseEntityRuntime entity, ContainerData cd)
            => cs.TryPut(entity, cd, EChangeOrigin.load);
    }

    /// Оркестратор сейвов/лоадов, единая точка входа.
    public sealed class SaveLoadService : ISaveLoadService
    {
        private readonly ISaveLoadStrategy _strategy;
        private readonly IContainmentService _containment;
        private readonly ICatalog _catalog;
        private readonly IEntityFactory<ItemRuntime, ItemDefinition> _itemFactory;
        private readonly IPlacementPolicy _placement;

        public SaveLoadService(
            ISaveLoadStrategy strategy,
            IPlacementPolicy placement = null
        )
        {
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            _containment = ServiceRegistry.Get<IContainmentService>();
            _catalog = ServiceRegistry.Get<ICatalog>();
            _itemFactory = ServiceRegistry.Get<IEntityFactory<ItemRuntime, ItemDefinition>>();
            _placement = placement ?? new StrictPlacementPolicy();
        }

        /*──────────── Save ────────────*/
        public void SaveGame()
        {
            // Берём все ISaveLoadObject (активные и неактивные)
            var participants = Object.FindObjectsOfType<MonoBehaviour>(includeInactive: true)
                .OfType<ISaveLoadObject>()
                .ToArray();

            _strategy.Save(participants);
            Debug.Log($"[Save] wrote {participants.Length} objects via {_strategy.GetType().Name}");
        }

        /*──────────── Load (чистая загрузка) ────────────*/
        public void LoadGameFresh()
        {
            var chunks = _strategy.Load() ?? Array.Empty<SaveLoadData>();

            // 0) Очистка: удаляем ВСЕ динамические сущности; инфраструктуру/владельцев не трогаем.
            ClearDynamicEntities();

            // 1) Разделяем на entity-чанки и component-чанки
            var entityChunks = new List<SaveLoadData>(capacity: chunks.Length);
            var nonEntityChunks = new List<SaveLoadData>(capacity: chunks.Length);
            foreach (var c in chunks)
                (IsEntityChunk(c) ? entityChunks : nonEntityChunks).Add(c);

            // 2) Спавним все entity (без раскладки по контейнерам)
            var createdById = new Dictionary<string, BaseEntityRuntime>(entityChunks.Count);

            using (_containment.SuppressEventsScope())
            {
                foreach (var ch in entityChunks)
                {
                    if (!TryReadDto(ch, out var dto)) continue;

                    if (string.IsNullOrEmpty(dto.definitionKey))
                    {
                        Debug.LogWarning($"[Load] chunk {ch.Id}: empty definitionKey");
                        continue;
                    }

                    // Пока поддерживаем Item; расширение — аналогично.
                    if (_catalog.TryGet<BigHeart.ItemDefinition>(dto.definitionKey, out var def))
                    {
                        var rt = _itemFactory.Create(def, Vector3.zero, Quaternion.identity);

                        // Восстановить StableId ДО применения стейта.
                        if (TryParseGuidLoose(dto.entityId, out var gid))
                            (rt.GetComponent<StableId>() ?? rt.gameObject.AddComponent<StableId>()).SetFromSave(gid);

                        // Применяем стейт и world-позу (если это World).
                        // Локацию Container НЕ трогаем здесь — это следующая фаза.
                        if (rt is ItemRuntime item)
                            item.RestoreFromDto(dto, initDefinition: false);
                        else
                            rt.GetType().GetMethod("RestoreFromDto")?.Invoke(rt, new object[]
                            {
                                dto,
                                false
                            });

                        createdById[rt.EntityId] = rt;
                    }
                    else
                    {
                        Debug.LogWarning($"[Load] chunk {ch.Id}: unknown definition '{dto.definitionKey}'");
                    }
                }

                // 3) ВТОРОЙ проход: раскладка по контейнерам
                foreach (var ch in entityChunks)
                {
                    if (!TryReadDto(ch, out var dto)) continue;
                    if (!createdById.TryGetValue(dto.entityId, out var entity)) continue;

                    if (dto.location.kind == EEntityLocationKind.container)
                    {
                        var cd = dto.location.container;
                        if (!_placement.TryPlace(_containment, entity, cd))
                            Debug.LogWarning(
                                $"[Load] place failed: {entity.EntityId} → {cd.ownerId}/{cd.containerKey}[{cd.index}]");
                    }
                    // World-позицию уже поставили в RestoreFromDto (выше).
                }
            }

            // 4) Рассылаем component-чанки адресно
            ApplyComponentChunks(nonEntityChunks);

            Debug.Log($"[Load] entities={createdById.Count}, componentChunks={nonEntityChunks.Count}");
        }

        /*──────────── Helpers ────────────*/

        private void ClearDynamicEntities()
        {
            var all = Object.FindObjectsOfType<BaseEntityRuntime>(includeInactive: true);
            foreach (var rt in all)
            {
                if (rt is IPersistentRuntime) continue; // игрок, сундуки-владельцы, инфраструктура
                Object.Destroy(rt.gameObject);
            }
        }

        private static bool IsEntityChunk(SaveLoadData c)
            => c.Type == ESaveType.entity
                || (!string.IsNullOrEmpty(c.Id) && c.Id.StartsWith("entity:", StringComparison.Ordinal));

        private static bool TryReadDto(SaveLoadData c, out EntitySaveData dto)
        {
            try
            {
                dto = c.Read<EntitySaveData>();
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                dto = default;
                return false;
            }
        }

        private static bool TryParseGuidLoose(string s, out Guid g)
        {
            if (Guid.TryParse(s, out g)) return true;
            if (!string.IsNullOrEmpty(s) && s.Length == 32)
                return Guid.TryParseExact(s, "N", out g);
            return false;
        }

        private static void ApplyComponentChunks(List<SaveLoadData> componentChunks)
        {
            // Карта "SaveId -> ISaveLoadObject". Если дубликаты — берём первый и логируем.
            var recipients = new Dictionary<string, ISaveLoadObject>(StringComparer.Ordinal);
            foreach (var o in Object.FindObjectsOfType<MonoBehaviour>(includeInactive: true)
                         .OfType<ISaveLoadObject>())
            {
                if (string.IsNullOrEmpty(o.ComponentSaveId)) continue;
                if (!recipients.TryAdd(o.ComponentSaveId, o))
                    Debug.LogWarning($"[Load] duplicate ComponentSaveId '{o.ComponentSaveId}' — keeping first");
            }

            var applied = 0;
            foreach (var c in componentChunks)
            {
                if (recipients.TryGetValue(c.Id, out var target))
                {
                    try
                    {
                        target.RestoreData(c);
                        applied++;
                    }
                    catch (Exception ex) { Debug.LogException(ex); }
                }
            }
            Debug.Log($"[Load] applied component chunks: {applied}/{componentChunks.Count}");
        }
    }
}
