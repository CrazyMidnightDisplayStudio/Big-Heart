using System;
using Base;
using UnityEngine;
using ItemSystem;

namespace Services
{
    /// <summary>
    /// Единый фасад для спавна, поиска и удаления предметов.
    /// Пока реализован как Singleton-MonoBehaviour, позже
    /// легко вынести в DI-контейнер, сохранив публичный интерфейс.
    /// </summary>
    public class ItemService : Service, IItemService
    {
        [Header("Generic prefab (fallback)")]
        [Tooltip("Если OverridePrefab в ItemDefinition = null, будет использован этот префаб")]
        [SerializeField]
        private GameObject genericItemPrefab;

        private readonly IEntityFactory<ItemPresenter, ItemDefinition> _factory; // Instantiate + Init
        private readonly EntityRegistry _registry; // глобальный реестр всех runtime-Id


        public ItemService(IEntityFactory<ItemPresenter, ItemDefinition> factory,
            EntityRegistry registry) : base("ItemService")
        {
            _factory = factory;
            _registry = registry;
        }

        /* ─────────────────────── API ──────────────────────────── */

        /// <summary>Создаёт предмет в сцене и возвращает обёртку-презентер.</summary>
        public ItemPresenter Create(ItemDefinition def,
            Vector3 pos = default,
            Transform parent = null)
        {
            if (def == null)
            {
                Debug.LogError("ItemService.Create: definition == null");
                return null;
            }

            return _factory.Spawn(def, pos, parent);
        }

        /// <summary>Удаляет предмет по его runtime-Id.</summary>
        public void Destroy(Guid runtimeId)
        {
            if (!_registry.TryGet<ItemPresenter>(runtimeId, out var item)) return;

            _registry.Unregister(item); // снимаем с учета
            UnityEngine.Object.Destroy(item.gameObject); // убираем из сцены
        }

        /// <summary>Проверяет наличие предмета с данным Id.</summary>
        public bool Exists(Guid runtimeId) =>
            _registry.TryGet<ItemPresenter>(runtimeId, out _);

        /// <summary>Пытается получить ссылку на ItemPresenter по Id.</summary>
        public bool TryGet(Guid runtimeId, out ItemPresenter presenter) =>
            _registry.TryGet(runtimeId, out presenter);
    }
}