using CMD.Core;
using CMD.Services;

namespace BigHeart.Services
{
    public class ItemCatalogService : Service, ICatalog<ItemDefinition>
    {
        private readonly ItemCatalogFromResources _itemCatalog;

        public ItemCatalogService() : base("ItemCatalog")
        {
            _itemCatalog = new ItemCatalogFromResources();
        }

        public ItemDefinition GetByKey(string key) => _itemCatalog.GetByKey(key);
        public bool TryGetByKey(string key, out ItemDefinition def) => _itemCatalog.TryGetByKey(key, out def);
    }
}
