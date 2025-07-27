using System;
using Base;
using Services;
using UnityEngine;

namespace ItemSystem
{
    public class ItemMono : MonoBehaviour, IEntity<ItemDefinition>
    {
        [SerializeField] private ItemDefinition itemDefinition;

        private Animator _animator;
        private IEffect _effect;

        public IEffect Effect => _effect;
        public ItemDefinition Definition => itemDefinition;
        public SlotType GetSlotType() => itemDefinition.slotType;

        public string Id { get; private set; }
        public string Tag => itemDefinition != null ? itemDefinition.itemTag : "UNKNOWN";

        public void Init(ItemDefinition def)
        {
            if (itemDefinition != null)
            {
                Debug.LogWarning($"{name} already initialised");
                return;
            }

            itemDefinition = def ?? throw new ArgumentNullException(nameof(def));
            Id = IdGenerator.NewId(); // уникален в сейве

            // 1. Аниматор
            _animator = GetComponent<Animator>();

            // 2. View
            var view = GetComponentInChildren<ItemView>();
            if (view) view.Init(def);

            // 3. Gameplay-эффект
            _effect = new CommonEffect(_animator, def);
        }

        /* ───── Optional: защита для объектов, лежащих в сцене дизайнером ───── */
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying && itemDefinition == null)
                Debug.LogWarning($"{name}: ItemDefinition missing");
        }
#endif
    }
}