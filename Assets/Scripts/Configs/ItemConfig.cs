using ItemSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace Configs
{
    [CreateAssetMenu(menuName = "Configs/ItemConfig", fileName = "NewItemConfig")]
    public class ItemConfig : BaseConfig
    {
        [Header("Основные параметры")]
        public string itemId;
        public string displayName;
        public SlotType slotType;
        public string description;
        public Sprite icon;
        public int effectValue;
        public float repeatIntervalTime;
        public float duration;
    }
}
