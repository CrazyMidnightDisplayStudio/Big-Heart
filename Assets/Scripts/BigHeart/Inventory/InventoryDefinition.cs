using UnityEngine;

namespace BigHeart.Inventory
{
    [CreateAssetMenu(menuName = "BigHeart/Inventory Definition", fileName = "InventoryDefinition")]
    public class InventoryDefinition : ScriptableObject
    {
        public string containerKey = "inventory";
        [Min(1)] public int columns = 5;
        [Min(1)] public int rows = 4;
        public Vector2 cellSize = new(64, 64);
        public Vector2 spacing = new(6, 6);
        public RectOffset padding;
    }
}
