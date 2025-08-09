// ItemView.cs

using Base;
using UnityEngine;

namespace ItemSystem
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ItemView : MonoBehaviour, IViewBinder<ItemDefinition>
    {
        private string _title;
        private string _description;

        public void Bind(ItemDefinition def)
        {
            if (!TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            {
                Debug.LogError($"{name}: ItemView requires a SpriteRenderer");
                return;
            }
            spriteRenderer.sprite = def.icon;
            _title = def.displayName;
            _description = def.description;
        }

        private void OnMouseOver()
        {
            TooltipScreenSpaceUI.ShowTooltip_Static(() => $"{_title}\n{_description}");
        }

        private void OnMouseExit() => TooltipScreenSpaceUI.HideTooltip_Static();
    }
}