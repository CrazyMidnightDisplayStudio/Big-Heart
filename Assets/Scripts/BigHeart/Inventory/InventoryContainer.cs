using System;
using System.Collections.Generic;
using CMD.Base;
using CMD.Common; // BaseEntityRuntime, StableId
using CMD.Core; // ServiceRegistry
using CMD.Services; // IContainmentService, IEntityContainer
using UnityEngine;

namespace BigHeart
{
    /// <summary>
    /// Универсальный контейнер: фиксированный массив слотов, 1 сущность на слот.
    /// Никаких событий здесь нет — только «чистые» операции хранения.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class InventoryContainer : MonoBehaviour, IEntityContainer
    {
        [Header("Owner & Identity")]
        [SerializeField] private StableId owner; // чей контейнер (стабильный id)
        [SerializeField] private string containerKey = "inventory";

        [Header("Capacity")]
        [SerializeField, Min(1)] private int capacity = 16;

        // Внутреннее хранилище (индекс == слот)
        private readonly List<BaseEntityRuntime> _items = new();

        // Сервис нужен только для регистрации/анрегистрации
        private IContainmentService _containment;

        /*──────────── IEntityContainer ───────────*/
        public string OwnerId
        {
            get
            {
                if (!owner) throw new InvalidOperationException("InventoryContainer: StableId(owner) missing.");
                return owner.IdString;
            }
        }

        public string ContainerKey => containerKey;
        public int Capacity => capacity;

        public bool Add(BaseEntityRuntime e, int index = -1, string slotKey = null)
        {
            if (!e) return false;
            EnsureCapacity();

            // не добавляем повторно
            if (_items.IndexOf(e) >= 0) return false;

            // выбрать первый свободный слот, если индекс не задан
            if (index < 0)
            {
                index = _items.FindIndex(x => x == null);
                if (index < 0) return false;
            }

            if (!InRange(index) || _items[index] != null) return false;

            _items[index] = e;
            return true;
        }

        public bool Remove(BaseEntityRuntime e)
        {
            if (!e) return false;
            EnsureCapacity();

            var i = _items.IndexOf(e);
            if (i < 0) return false;

            _items[i] = null;
            return true;
        }

        /// <summary>
        /// Перенос в ПУСТОЙ слот (свапа нет — это упрощает инварианты).
        /// </summary>
        public bool Move(BaseEntityRuntime e, int newIndex, string newSlotKey = null)
        {
            if (!e) return false;
            EnsureCapacity();

            var from = _items.IndexOf(e);
            if (from < 0) return false;
            if (!InRange(newIndex)) return false;
            if (_items[newIndex] != null) return false;

            _items[from] = null;
            _items[newIndex] = e;
            return true;
        }

        public bool Contains(BaseEntityRuntime e) => e && _items.IndexOf(e) >= 0;
        public int IndexOf(BaseEntityRuntime e) => e ? _items.IndexOf(e) : -1;

        public BaseEntityRuntime IndexOfSlot(int index)
        {
            EnsureCapacity();
            return InRange(index) ? _items[index] : null;
        }

        /*──────────── lifecycle ───────────*/
        private void Awake()
        {
            EnsureCapacity();
            _containment = ServiceRegistry.Get<IContainmentService>();
        }

        private void OnEnable() => _containment?.Register(this);
        private void OnDisable() => _containment?.Unregister(this);

        private void OnValidate()
        {
            if (capacity < 1) capacity = 1;
            EnsureCapacity();
        }

        /*──────────── helpers ───────────*/
        private void EnsureCapacity()
        {
            while (_items.Count < capacity) _items.Add(null);
            if (_items.Count > capacity) _items.RemoveRange(capacity, _items.Count - capacity);
        }

        private bool InRange(int i) => i >= 0 && i < capacity;

        // (опционально) отладочный доступ
        public IReadOnlyList<BaseEntityRuntime> Slots => _items;
    }
}
