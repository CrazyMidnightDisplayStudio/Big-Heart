using System;
using System.Collections.Generic;
using Entities.Base;
using Entities.Item.Model;
using Entities.Item.Presenter;
using UnityEngine;

namespace Services
{
    /// <summary>
    /// Единый фасад для спавна, поиска и удаления предметов.
    /// </summary>
    public class DebugItemSpawnerService : Service, IItemService
    {
        [Header("Generic prefab (fallback)")]
        [Tooltip("Если OverridePrefab в ItemDefinition = null, будет использован этот префаб")]
        [SerializeField]
        private GameObject genericItemPrefab;

        private readonly IPresenterFactory<ItemPresenter> _factory;
        private readonly Dictionary<Guid, ItemPresenter> _items;


        public DebugItemSpawnerService(IPresenterFactory<ItemPresenter> factory) : base("ItemService")
        {
            _factory = factory;
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

            return _factory.Spawn(def, pos, parent, Guid.NewGuid(), new ItemState());
        }

        /// <summary>Удаляет предмет по его runtime-Id.</summary>
        public void Destroy(Guid runtimeId)
        {
            if (!_items.TryGetValue(runtimeId, out ItemPresenter item)) return;

            _items.Remove(runtimeId);
            UnityEngine.Object.Destroy(item.gameObject); // убираем из сцены
        }

        /// <summary>Проверяет наличие предмета с данным Id.</summary>
        public bool Exists(Guid runtimeId) => _items.ContainsKey(runtimeId);

        /// <summary>Пытается получить ссылку на ItemPresenter по Id.</summary>
        public bool TryGet(Guid runtimeId, out ItemPresenter presenter) =>
            _items.TryGetValue(runtimeId, out presenter);
    }
}
