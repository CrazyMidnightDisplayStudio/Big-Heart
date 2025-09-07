using CatalogKeys;
using CMD.Base;
using CMD.Core;
using CMD.Services;
using UnityEngine;

namespace BigHeart
{
    [CreateAssetMenu(menuName = "BigHeart/Item Definition", fileName = "Item_")]
    public class ItemDefinition : EntityDefinition, IDisplayInfo, IPrefabProvider
    {
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }

        [Header("Prefab")]
        [SerializeField] private GameObject prefab;
        public GameObject Prefab
        {
            get
            {
                if (prefab) { return prefab; }
                var genericPrefab = ServiceRegistry.Get<ICatalog>().Get<UnityEngine.GameObject>(CMDCatalog.Prefabs.GenericItem);
                return genericPrefab;
            }
        }
    }
}
