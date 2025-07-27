using Configs;
using Services;
using UnityEngine;

namespace ItemSystem
{
    public class CommonEffect : IEffect
    {
        private Animator _animator;
        private ItemDefinition _itemDefinition;
        
        public bool IsActive { get; private set; }
        
        public CommonEffect(Animator animator, ItemDefinition definition)
        {
            _animator = animator;
            _itemDefinition = definition;
        }
        
        public void OnEquip()
        {
        }

        public void OnUnEquip()
        {
        }

        public void OnDateStart()
        {
            IsActive = true;
            CoroutineService.Instance.RunRepeatingCoroutine(PeriodicEffect, _itemDefinition.repeatIntervalTime, () => !IsActive);
        }

        public void OnDateEnd()
        {
            IsActive = false;
        }

        public void PeriodicEffect()
        {
            EventService.Instance.OnAddPositiveEffect(_itemDefinition.effectValue);
        }
    }
}