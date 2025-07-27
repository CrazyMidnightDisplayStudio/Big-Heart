using Base;
using UnityEngine;

namespace ItemSystem
{
    public class ItemView : MonoBehaviour, IViewMarker
    {
        [SerializeField] private string attributeName;
        [SerializeField] private string description;

        private ItemDefinition _itemDefinition;
        
        public void Init(ItemDefinition itemDefinition)
        {
            _itemDefinition = itemDefinition;
            attributeName = itemDefinition.displayName;
            description = itemDefinition.description;
        }
        
        private void OnMouseOver()
        {
            System.Func<string> getTooltipTextFunc = () => $"{attributeName} \n{description}";
            TooltipScreenSpaceUI.ShowTooltip_Static(getTooltipTextFunc);
        }

        private void OnMouseExit()
        {
            TooltipScreenSpaceUI.HideTooltip_Static();
        }
    }
}