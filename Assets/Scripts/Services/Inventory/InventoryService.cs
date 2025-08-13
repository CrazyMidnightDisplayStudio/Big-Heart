using System.Collections.Generic;
using Entities.Item.Presenter;

namespace Services.Inventory
{
    public sealed class InventoryService : Service, IInventoryService
    {
        readonly List<ItemPresenter> _equipped = new();

        public InventoryService() : base("InventoryService") { }

        public void Equip(ItemPresenter item)
        {
            _equipped.Add(item);
            // контейнер уже подписан на события и будет активен при DateStarted
        }

        public IEnumerable<ItemPresenter> Equipped => _equipped;
    }
}
