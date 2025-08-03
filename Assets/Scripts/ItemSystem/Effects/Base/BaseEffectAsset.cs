using UnityEngine;

namespace ItemSystem.Effects
{
// абстрактный SO, который знает, как построить runtime-эффект
    public abstract class BaseEffectAsset : ScriptableObject, IEffect
    {
        public virtual IEffect BuildRuntime(ItemModel owner) => Instantiate(this);
    }
}
