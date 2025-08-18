using UnityEngine;

namespace CMD.Entities
{
    [CreateAssetMenu(menuName="CMD.Core/Containers/Grid Definition")]
    public sealed class GridContainerDefinitionSO : ScriptableObject
    {
        public string containerKey = "Backpack";
        public int rows = 4;
        public int cols = 6;
        public Vector2 cellSize = new(64, 64);
        public Vector2 padding = new(8, 8);
    }
}
