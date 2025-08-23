using CMD.Core;
using CMD.Events.ContainmentEvents;
using CMD.Services;
using UnityEngine;

namespace BigHeart.Inventory
{
    public sealed class InventoryPresenter : MonoBehaviour
    {
        [Header("Wiring")]
        [SerializeField] private InventoryContainer container; // данные
        [SerializeField] private InventoryDefinition definition; // конфиг сетки
        [SerializeField] private InventoryView view; // визуал

        private IEventBusService _bus;
        private IContainmentService _containment;
        private System.IDisposable _dAdded, _dRemoved, _dMoved;

        private void Awake()
        {
            _bus = ServiceRegistry.Get<IEventBusService>();
            _containment = ServiceRegistry.Get<IContainmentService>();
            view.BuildGrid(definition);
            FullRefresh();
        }

        private void OnEnable()
        {
            // фильтр по владельцу и ключу контейнера
            _dAdded = _bus.Subscribe<AddedToContainer>(
                e => { UpdateSlot(e.Index); },
                e => e.OwnerId == container.OwnerId && e.ContainerKey == container.ContainerKey);

            _dRemoved = _bus.Subscribe<RemovedFromContainer>(
                _ => FullRefresh(), // если у тебя есть индекс в событии — обнови точечно
                e => e.OwnerId == container.OwnerId && e.ContainerKey == container.ContainerKey);

            _dMoved = _bus.Subscribe<MovedIntoContainer>(
                e =>
                {
                    UpdateSlot(e.FromIndex);
                    UpdateSlot(e.ToIndex);
                },
                e => e.OwnerId == container.OwnerId && e.ContainerKey == container.ContainerKey);
        }

        private void OnDisable()
        {
            _dAdded?.Dispose();
            _dRemoved?.Dispose();
            _dMoved?.Dispose();
        }

        private void FullRefresh()
        {
            view.ClearAll();
            var cols = definition.columns;
            var rows = definition.rows;
            for (int i = 0; i < cols * rows; i++) UpdateSlot(i);
        }

        private void UpdateSlot(int index)
        {
            var rt = container.IndexOfSlot(index);
            Sprite icon = null;

            // иконка есть только у ItemDefinition — проверяем тип
            if (rt && rt.Definition is BigHeart.ItemDefinition idef)
                icon = idef.Icon;

            view.SetIcon(index, icon);
        }
    }
}
