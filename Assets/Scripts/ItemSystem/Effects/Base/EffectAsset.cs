using UnityEngine;

namespace ItemSystem.Effects
{
// абстрактный SO, который знает, как построить runtime-эффект
    public abstract class EffectAsset : ScriptableObject
    {
        // public abstract IEffect BuildRuntime(ItemMonoEntity owner);
    }
}