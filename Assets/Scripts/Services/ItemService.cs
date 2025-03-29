using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ItemSystem;
using UnityEngine;

namespace Services
{
    public class ItemService : BaseServiceSingleton<ItemService>
    {
        private Dictionary<string, ItemMono> _itemsById;
        private ItemFactory _itemFactory;
        
        public override void Init()
        {
            base.Init();
            _itemsById = new Dictionary<string, ItemMono>();
            _itemFactory = new ItemFactory();
            IsInitialized = true;
            Debug.Log("ItemService initialized");
        }

        public async Task<ItemMono> CreateItem(string itemId, Vector3 position = default, Transform parent = null)
        {
            if (_itemsById.TryGetValue(itemId, out var existingItem))
            {
                if (parent == null || existingItem.transform.parent == parent)
                {
                    Debug.LogWarning($"[ItemService] Предмет {itemId} уже существует в {parent?.name ?? "сцене"}.");
                    return existingItem;
                }
            }

            try
            {
                var item = await _itemFactory.CreateItemAsync(itemId, position, parent);
                
                if (item == null)
                {
                    Debug.LogError($"[ItemService] Не удалось создать предмет {itemId}, item == null.");
                    return null;
                }

                RegisterItem(item);
                return item;
            }
            catch (Exception e)
            {
                Debug.LogError($"[ItemService] Ошибка создания предмета {itemId}: {e}");
                return null;
            }
        }

        public void DestroyItem(string itemId)
        {
            if (_itemsById.TryGetValue(itemId, out var item))
            {
                UnregisterItem(itemId);
                _itemsById.Remove(itemId);
                Destroy(item.gameObject);
                Debug.Log($"[ItemService] Предмет {itemId} удалён.");
            }
        }

        public ItemMono GetItem(string itemId) => _itemsById.TryGetValue(itemId, out var item) ? item : null;

        public bool ContainsItem(string itemId)
        {
            return _itemsById.ContainsKey(itemId);
        }
        
        private void RegisterItem(ItemMono item)
        {
            if (item == null || string.IsNullOrEmpty(item.ItemId)) return;

            if (!_itemsById.ContainsKey(item.ItemId))
            {
                _itemsById[item.ItemId] = item;
                Debug.Log($"[ItemService] Предмет {item.ItemId} добавлен в список регистраций.");
            }
            else
            {
                Debug.Log($"[ItemService] Предмет {item.ItemId} уже есть в списке регистраций.");
            }
        }

        private void UnregisterItem(string itemId)
        {
            if (_itemsById.TryGetValue(itemId, out ItemMono item))
            {
                _itemsById.Remove(itemId);
                Debug.Log($"[ItemService] Предмет {itemId} убран из списка регистраций.");
            }
        }
    }
}