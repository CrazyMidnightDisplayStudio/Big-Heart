using Configs;
using Services;
using UnityEngine;

namespace ItemSystem
{
    public class ItemMono : MonoBehaviour
    {
        [SerializeField] private ItemConfig itemConfig;
        
        private Animator _animator;
        private IEffect _effect;
        private ItemService _itemService;
        
        public IEffect Effect => _effect;
        public ItemConfig Config => itemConfig;
        public SlotType GetSlotType() => itemConfig.slotType;
        public string ItemId => itemConfig != null ? itemConfig.itemId : "UNKNOWN";

        private void Awake()
        {
            if (itemConfig == null)
            {
                Debug.LogError($"[BaseItemMono] У предмета {gameObject.name} не назначен ItemConfig!");
                return;
            }

            _animator = GetComponent<Animator>();
            var view = GetComponentInChildren<ItemView>();
            if (view != null)
            {
                view.Init(itemConfig);
            }
            _effect = new CommonEffect(_animator, itemConfig);
        }
    }
}