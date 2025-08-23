using UnityEngine;
namespace BigHeart
{
    /// <summary>Есть отображаемая информация.</summary>
    public interface IDisplayInfo
    {
        public Sprite Icon { get; }
        public string DisplayName { get; }
        public string Description { get; }
    }

}
