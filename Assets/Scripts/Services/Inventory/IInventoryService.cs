using System.Collections.Generic;
using ItemSystem;

namespace Services.Inventory
{
    public interface IInventoryService
    {
        public IEnumerable<ItemPresenter> Equipped { get; }
        public void Equip(ItemPresenter item);
    }
}