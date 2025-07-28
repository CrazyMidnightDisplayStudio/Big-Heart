using System.Collections.Generic;
using Base;
using ItemSystem;
using SaveLoadSystem;
using UnityEngine;

namespace Services
{
    public class InventoryService : BaseServiceSingleton<InventoryService>
    {
        public string SaveKey => "Inventory";
        
        private float _placementRadius;
        
        // это нужно только для того, чтобы следить за размером инвентаря
        private Dictionary<SlotType, List<ItemMonoEntity>> _itemsBySlot;
        private Dictionary<SlotType, int> _inventoryCapacity;

        // а вот тут уже сами айтемы списком.
        // SerializeField только для наглядности в инспекторе, его нельзя менять через инспектор, только смотреть
        [SerializeField] private List<ItemMonoEntity> _items;
        
        public List<ItemMonoEntity> Items => _items;

        public override void Init()
        {
            base.Init();
            _placementRadius = ConfigService.Instance.inventoryConfig.placementRadius;
            _inventoryCapacity = ConfigService.Instance.inventoryConfig.ToDictionary();
            _itemsBySlot = new Dictionary<SlotType, List<ItemMonoEntity>>();
            foreach (var slotType in _inventoryCapacity.Keys)
            {
                _itemsBySlot[slotType] = new List<ItemMonoEntity>();
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
            if (EntityRegistry.Instance.TryGet<ItemMonoEntity>(itemId, out var item))
            {
                EquipItem(item);
                return true;
            }

            return false;
        }

        public bool EquipItem(ItemMonoEntity itemMono)
        {
            if (itemMono == null)
            {
                Debug.LogWarning("Item is null");
                return false;
            }
            
            var equippedItemsCount = _itemsBySlot[itemMono.GetSlotType()].Count;
            var capacityForItem = _inventoryCapacity[itemMono.GetSlotType()];
            if (equippedItemsCount < capacityForItem)
            {
                _itemsBySlot[itemMono.GetSlotType()].Add(itemMono);
                _items.Add(itemMono);
                itemMono.Effect.OnEquip();
                ArrangeItemsInCircle();
                return true;
            }

            return false;
        }

        public bool UnEquipItem(ItemMonoEntity itemMono)
        {
            var equippedItemsCount = _itemsBySlot[itemMono.GetSlotType()].Count;
            if (equippedItemsCount > 0)
            {
                _itemsBySlot[itemMono.GetSlotType()].Remove(itemMono);
                _items.Remove(itemMono);
                itemMono.Effect.OnUnEquip();
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
                items.Add(item.Tag);
            }

            return new InventorySaveData { items = items };
        }
    }
    
    [System.Serializable]
    public class InventorySaveData
    {
        public List<string> items;
    }
}