using Base;
using UnityEngine;

namespace ItemSystem
{
    public class ItemView : MonoBehaviour, IViewBinder<ItemDefinition>
    {
        private string _attributeName;
        private string _description;

        private ItemDefinition _itemDefinition;
        public Sprite GetSprite => _itemDefinition.icon;
        
        public void Bind(ItemDefinition definition)
        {
            _itemDefinition = definition;
            _attributeName = definition.displayName;
            _description = definition.description;
        }
        
        private void OnMouseOver()
        {
            System.Func<string> getTooltipTextFunc = () => $"{_attributeName} \n{_description}";
            TooltipScreenSpaceUI.ShowTooltip_Static(getTooltipTextFunc);
        }

        private void OnMouseExit()
        {
            TooltipScreenSpaceUI.HideTooltip_Static();
        }

    }
}