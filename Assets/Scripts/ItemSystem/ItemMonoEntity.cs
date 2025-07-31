// using System;
// using Base;
// using ItemSystem.Effects;
// using UnityEngine;
//
// namespace ItemSystem
// {
//     
//     [RequireComponent(typeof(EffectContainer))]
//     public class ItemMonoEntity : MonoBehaviour, IEntity<ItemDefinition>
//     {
//         [SerializeField] private ItemDefinition itemDefinition;
//
//         private ItemView _view;
//         private IEffect _effect;
//         private EffectContainer _effects;
//
//         public IEffect Effect => _effect;
//         public ItemDefinition Definition => itemDefinition;
//         public SlotType GetSlotType() => itemDefinition.slotType;
//
//         public string Id { get; private set; }
//         public string Tag => itemDefinition != null ? itemDefinition.itemTag : "UNKNOWN";
//
//         public void Init(ItemDefinition def)
//         {
//             if (itemDefinition != null)
//             {
//                 Debug.LogError($"{name} already initialised");
//                 return;
//             }
//
//             itemDefinition = def ?? throw new ArgumentNullException(nameof(def));
//             Id = IdGenerator.NewId(); // уникален в сейве
//
//             // 1. View
//             _view = gameObject.AddComponent<ItemView>();
//             _view.Bind(def);
//
//             // 2. Gameplay-эффект
//             _effects = GetComponent<EffectContainer>();
//             _effects.Init(this, def.effects);
//         }
//         
//         /*────────────────── Проксируем геймплейные события ───────────*/
//         public void HandleEquip()     => _effects.OnEquip();
//         public void HandleUnEquip()   => _effects.OnUnEquip();
//         public void HandleDateStart() => _effects.OnDateStart();
//         public void HandleDateEnd()   => _effects.OnDateEnd();
//
//         /*──────────────────────────── Cleanup ────────────────────────*/
//         private void OnDestroy() => EntityRegistry.Instance.Unregister(this);
//
//         /* ───── Optional: защита для объектов, лежащих в сцене дизайнером ───── */
// #if UNITY_EDITOR
//         private void OnValidate()
//         {
//             if (!Application.isPlaying && itemDefinition == null)
//                 Debug.LogWarning($"{name}: ItemDefinition missing");
//         }
// #endif
//     }
// }