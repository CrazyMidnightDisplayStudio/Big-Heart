using CMD.Base;
using UnityEngine;

namespace BigHeart
{
    [CreateAssetMenu(menuName = "BigHeart/Item Definition", fileName = "Item_")]
    public class ItemDefinition : EntityDefinitionSO, IDisplayInfo, IPrefabProvider
    {
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }

        [Header("Prefab")]
        [SerializeField] private GameObject prefab;
        public GameObject Prefab => prefab;
    }
}
