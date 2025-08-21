using System;
using System.Collections.Generic;
using CMD.Core;
using CMD.Entities;
using CMD.Services;
using UnityEngine;

namespace BigHeart
{
    [DisallowMultipleComponent]
    public sealed class InventoryContainer : MonoBehaviour, IEntityContainer
    {
        [SerializeField] private string ownerId = "Player";
        [SerializeField] private GridContainerDefinitionSO definition;
        [SerializeField] private RectTransform slotsRoot; // куда ставить предметы в UI (может быть обычный Transform)

        private readonly List<BaseEntityRuntime> _items = new(); // индекс = слот

        public string OwnerId => ownerId;
        public string ContainerKey => definition ? definition.containerKey : throw new InvalidOperationException();
        public int Capacity => (definition ? definition.rows * definition.cols : 0);

        private IContainmentService containmentService;

        private void Awake()
        {
            EnsureCapacity();
            containmentService = ServiceRegistry.Get<IContainmentService>();
        }
        private void OnEnable() => containmentService?.Register(this);
        private void OnDisable() => containmentService?.Unregister(this);

        private void EnsureCapacity()
        {
            var need = Capacity;
            while (_items.Count < need) _items.Add(null);
            if (_items.Count > need) _items.RemoveRange(need, _items.Count - need);
        }

        // IEntityContainer
        public bool Add(BaseEntityRuntime e, int index = -1, string slotKey = null)
        {
            if (e == null) return false;
            EnsureCapacity();
            if (index < 0)
            {
                index = _items.FindIndex(x => x == null);
                if (index < 0) return false;
            }
            if (index >= _items.Count || _items[index] != null) return false;

            _items[index] = e;
            AttachView(e, index);
            // событие для триггеров/систем UI
            (containmentService as ContainmentService)?.OnAdded(e, this, index);
            return true;
        }

        public bool Remove(BaseEntityRuntime e)
        {
            if (e == null) return false;
            var i = _items.IndexOf(e);
            if (i < 0) return false;
            _items[i] = null;
            (containmentService as ContainmentService)?.OnRemoved(e, this);
            return true;
        }

        public bool Move(BaseEntityRuntime e, int newIndex, string newSlotKey = null)
        {
            var i = _items.IndexOf(e);
            if (i < 0) return false;
            if (newIndex < 0 || newIndex >= _items.Count) return false;
            if (_items[newIndex] != null) return false;

            _items[i] = null;
            _items[newIndex] = e;
            AttachView(e, newIndex);
            (containmentService as ContainmentService)?.OnMoved(e, this, IndexOf(e));
            return true;
        }

        public bool Contains(BaseEntityRuntime e) => _items.IndexOf(e) >= 0;
        public int IndexOf(BaseEntityRuntime e) => _items.IndexOf(e);
        public BaseEntityRuntime IndexOfSlot(int index) => _items[index];

        // Визуальное позиционирование в UI/мире
        void AttachView(BaseEntityRuntime e, int index)
        {
            if (slotsRoot == null) return;
            var t = e.transform as RectTransform ?? e.GetComponent<RectTransform>();
            if (t == null) t = e.gameObject.AddComponent<RectTransform>();
            t.SetParent(slotsRoot, worldPositionStays: false);

            var (row, col) = IndexToRC(index);
            var pos = RCToLocalPos(row, col);
            t.anchoredPosition = pos;
            t.localScale = Vector3.one;
        }

        (int r, int c) IndexToRC(int index)
        {
            var cols = Mathf.Max(1, definition.cols);
            return (index / cols, index % cols);
        }

        Vector2 RCToLocalPos(int r, int c)
        {
            var size = definition.cellSize;
            var pad = definition.padding;
            // простой левый-верхний грид:
            var x = pad.x + c * size.x;
            var y = -(pad.y + r * size.y);
            return new Vector2(x, y);
        }
    }
}
