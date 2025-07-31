// ItemView.cs

using Base;
using UnityEngine;

namespace ItemSystem
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ItemView : MonoBehaviour, IViewBinder<ItemDefinition>
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private string _title;
        private string _description;


        public void Bind(ItemDefinition def)
        {
            // подстраховка
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (spriteRenderer == null)
            {
                Debug.LogError($"{name}: ItemView requires a SpriteRenderer");
                return; // выходим, чтобы не ловить NRE
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