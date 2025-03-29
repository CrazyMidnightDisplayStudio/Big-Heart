using System;
using System.Collections.Generic;
using ItemSystem;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(menuName = "Configs/InventoryConfig", fileName = "InventoryConfig")]
    public class InventoryConfig : BaseConfig
    {
        [Header("Укажите вместимость для каждого типа слота")]
        public SlotCapacityEntry[] slotCapacities;
        
        [Header("Радиус в котором будут распологаться предметы")]
        public float placementRadius = 2.5f;

        [Serializable]
        public struct SlotCapacityEntry
        {
            public SlotType slotType;
            public int capacity;
        }
        
        public Dictionary<SlotType, int> ToDictionary()
        {
            var dict = new Dictionary<SlotType, int>();
            foreach (var entry in slotCapacities)
            {
                dict[entry.slotType] = entry.capacity;
            }
            return dict;
        }
    }
}