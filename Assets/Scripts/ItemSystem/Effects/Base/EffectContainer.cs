using System;
using System.Collections.Generic;
using Services;
using UnityEngine;

namespace ItemSystem.Effects
{
    public sealed class EffectContainer : MonoBehaviour
    {
        readonly List<IEffect> _effects = new();
        readonly List<uint>  _periodicCoroutineIds = new();
        
        // ItemMonoEntity _owner;
        //
        // public void Init(ItemMonoEntity owner, IEnumerable<EffectAsset> assets)
        // {
        //     _owner = owner;
        //
        //     foreach (var asset in assets)
        //     {
        //         if (!asset) continue;
        //
        //         var runtime = asset.BuildRuntime(owner);
        //         _effects.Add(runtime);
        //
        //         if (runtime is IPeriodicEffect effect)
        //         {
        //             var stopCondition = new Func<bool>(() => owner == null || !owner.gameObject);
        //             
        //             _periodicCoroutineIds.Add(CoroutineService.Instance.RunRepeatingCoroutine(() => effect.Tick(), effect.IntervalSec, stopCondition));
        //         }
        //     }
        // }
        
        /* Проксируем хуки */
        public void OnEquip()
        {
            foreach (var e in _effects) e.OnEquip();
        }
        
        public void OnUnEquip()
        {
            foreach (var e in _effects) e.OnUnEquip();
        }
        
        public void OnDateStart()
        {
            foreach (var e in _effects) e.OnDateStart();
        }
        
        public void OnDateEnd()
        {
            foreach (var e in _effects) e.OnDateEnd();
        }
        
        void OnDestroy()
        {
            CoroutineService.Instance.Stop(_periodicCoroutineIds);
        }
    }
}