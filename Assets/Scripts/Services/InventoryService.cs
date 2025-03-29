using System.Collections.Generic;
using ItemSystem;
using SaveLoadSystem;
using UnityEngine;

namespace Services
{
    public class InventoryService : BaseServiceSingleton<InventoryService>, ISaveLoadObject
    {
        public string SaveKey => "Inventory";
        
        private float _placementRadius;
        
        // это нужно только для того, чтобы следить за размером инвентаря
        private Dictionary<SlotType, List<ItemMono>> _itemsBySlot;
        private Dictionary<SlotType, int> _inventoryCapacity;

        // а вот тут уже сами айтемы списком.
        // SerializeField только для наглядности в инспекторе, его нельзя менять через инспектор, только смотреть
        [SerializeField] private List<ItemMono> _items;
        
        public List<ItemMono> Items => _items;

        public override void Init()
        {
            base.Init();
            _placementRadius = ConfigService.Instance.inventoryConfig.placementRadius;
            _inventoryCapacity = ConfigService.Instance.inventoryConfig.ToDictionary();
            _itemsBySlot = new Dictionary<SlotType, List<ItemMono>>();
            foreach (var slotType in _inventoryCapacity.Keys)
            {
                _itemsBySlot[slotType] = new List<ItemMono>();
            }
            Debug.Log("InventoryService initialized");
        }
        
        private void ArrangeItemsInCircle()
        {
            int itemCount = _items.Count;
            if (itemCount == 0) return;

            float angleStep = 360f / itemCount; // Угол между предметами
            float startAngle = 0f; // Начинаем с правой стороны

            for (int i = 0; i < itemCount; i++)
            {
                float angle = startAngle + (angleStep * i); // Угол для каждого объекта
                float radians = angle * Mathf.Deg2Rad; // Конвертируем в радианы

                Vector3 newPosition = new Vector3(
                    transform.position.x + Mathf.Cos(radians) * _placementRadius, // X с учётом радиуса
                    transform.position.y + Mathf.Sin(radians) * _placementRadius, // Y с учётом радиуса
                    transform.position.z // Z остаётся тем же (2D игра)
                );

                _items[i].transform.position = newPosition;
            }
        }

        public bool EquipItem(string itemId)
        {
            if (!ItemService.Instance.ContainsItem(itemId))
            {
                return false;
            }
            
            EquipItem(ItemService.Instance.GetItem(itemId));
            return true;
        }

        public bool EquipItem(ItemMono item)
        {
            if (item == null)
            {
                Debug.LogWarning("Item is null");
                return false;
            }
            
            var equippedItemsCount = _itemsBySlot[item.GetSlotType()].Count;
            var capacityForItem = _inventoryCapacity[item.GetSlotType()];
            if (equippedItemsCount < capacityForItem)
            {
                _itemsBySlot[item.GetSlotType()].Add(item);
                _items.Add(item);
                item.Effect.OnEquip();
                ArrangeItemsInCircle();
                return true;
            }

            return false;
        }

        public bool UnEquipItem(ItemMono item)
        {
            var equippedItemsCount = _itemsBySlot[item.GetSlotType()].Count;
            if (equippedItemsCount > 0)
            {
                _itemsBySlot[item.GetSlotType()].Remove(item);
                _items.Remove(item);
                item.Effect.OnUnEquip();
                ArrangeItemsInCircle();
                return true;
            }
            return false;
        }

        public object GetSaveData()
        {
            List<string> items = new List<string>();
            foreach (var item in _items)
            {
                items.Add(item.ItemId);
            }

            return new InventorySaveData { items = items };
        }

        public void LoadFromSaveData(object data)
        {
            if (data is not InventorySaveData saveData)
            {
                Debug.LogError("Invalid save data");
                return;
            }
            
            foreach (var itemId in saveData.items)
            {
                ItemService.Instance.CreateItem(itemId);
                EquipItem(ItemService.Instance.GetItem(itemId));
            }
        }
    }
    
    [System.Serializable]
    public class InventorySaveData
    {
        public List<string> items;
    }
}