using System.Collections.Generic;
using UnityEngine;
using Base;
using ItemSystem.Effects;

namespace ItemSystem
{
    [CreateAssetMenu(menuName = "Entities/Item Definition", fileName = "NewItemDefinition")]
    public class ItemDefinition : BaseEntityDefinition
    {
        [Header("Display")]
        public string displayName;
        [TextArea(3, 5)] public string description;
        public Sprite icon;

        [Header("Gameplay")]
        public string itemTag;
        public SlotType slotType;
        public List<BaseEffectAsset> effects;

        [Header("Prefab (optional)")]
        [Tooltip("Оставь пустым, если годится Generic-префаб")]
        public GameObject overridePrefab;
    }
}
