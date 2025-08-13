using System.Collections.Generic;
using Entities.Item.Presenter;
namespace Services.Inventory
{
    public class StubInventoryService : IInventoryService
    {

        public IEnumerable<ItemPresenter> Equipped => new List<ItemPresenter>();

        public void Equip(ItemPresenter item)
        {
        }
    }
}
