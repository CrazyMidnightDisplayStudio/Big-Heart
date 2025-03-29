using Configs;
using Services;
using UnityEngine;

namespace ItemSystem
{
    public class CommonEffect : IEffect
    {
        private Animator _animator;
        private ItemConfig _itemConfig;
        
        public bool IsActive { get; private set; }
        
        public CommonEffect(Animator animator, ItemConfig config)
        {
            _animator = animator;
            _itemConfig = config;
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
            CoroutineService.Instance.RunRepeatingCoroutine(PeriodicEffect, _itemConfig.repeatIntervalTime, () => !IsActive);
        }

        public void OnDateEnd()
        {
            IsActive = false;
        }

        public void PeriodicEffect()
        {
            EventService.Instance.OnAddPositiveEffect(_itemConfig.effectValue);
        }
    }
}