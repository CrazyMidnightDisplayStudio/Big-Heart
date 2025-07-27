using UnityEngine;
using Base;

namespace ItemSystem
{
    
    [CreateAssetMenu(menuName = "Entities/ItemDefinition", fileName = "NewItemDefinition")]
    public class ItemDefinition : BaseEntityDefinition
    {
        [Header("BaseParams")]
        [Tooltip("Категория или групповой фильтр (не уникальный)")]
        public string itemTag;

        [Header("View")]
        public string displayName;
        public SlotType slotType;
        [TextArea(3, 5)]
        public string description;

        [Header("Sprite")]
        public Sprite icon;

        [Header("Gameplay")]
        public int   effectValue;
        public float repeatIntervalTime;
        public float duration;
    }
}
