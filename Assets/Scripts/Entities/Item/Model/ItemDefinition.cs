using System.Collections.Generic;
using Entities.Base;
using Entities.Item.Effect.Base;
using UnityEngine;

namespace Entities.Item.Model
{
    [CreateAssetMenu(menuName = "Content/Item Definition", fileName = "NewItemDefinition")]
    public class ItemDefinition : BaseEntityDefinition
    {
        [Header("Display")] public string displayName;
        [TextArea(3,5)] public string description;
        public Sprite icon;

        [Header("Gameplay")] public string itemTag;
        public string slotType;

        [Header("Effects")] public List<BaseEffectAsset> effects = new();
    }
}
