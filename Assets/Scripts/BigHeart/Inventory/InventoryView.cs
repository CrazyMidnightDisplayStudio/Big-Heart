using UnityEngine;
using UnityEngine.UI;

namespace BigHeart.Inventory
{
    public sealed class InventoryView : MonoBehaviour
    {
        [SerializeField] private RectTransform gridRoot;    // объект с GridLayoutGroup
        [SerializeField] private GameObject slotPrefab;     // внутри Image для иконки

        private Image[] _icons;

        public void BuildGrid(InventoryDefinition layout)
        {
            var gl = gridRoot.GetComponent<GridLayoutGroup>();
            gl.cellSize = layout.cellSize;
            gl.spacing  = layout.spacing;
            gl.padding  = layout.padding;

            var total = layout.rows * layout.columns;
            foreach (Transform c in gridRoot) Destroy(c.gameObject);

            _icons = new Image[total];
            for (int i = 0; i < total; i++)
            {
                var go = Instantiate(slotPrefab, gridRoot);
                _icons[i] = go.GetComponentInChildren<Image>(true);
                SetIcon(i, null);
            }
        }

        public void SetIcon(int index, Sprite icon)
        {
            if (_icons == null || index < 0 || index >= _icons.Length) return;
            _icons[index].enabled = icon != null;
            _icons[index].sprite  = icon;
        }

        public void ClearAll()
        {
            if (_icons == null) return;
            for (int i = 0; i < _icons.Length; i++) SetIcon(i, null);
        }
    }
}
