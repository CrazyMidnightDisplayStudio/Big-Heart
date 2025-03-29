using Configs;
using UnityEngine;

namespace ItemSystem
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private string attributeName;
        [SerializeField] private string description;

        private ItemConfig _itemConfig;
        
        public void Init(ItemConfig itemConfig)
        {
            _itemConfig = itemConfig;
            attributeName = itemConfig.displayName;
            description = itemConfig.description;
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