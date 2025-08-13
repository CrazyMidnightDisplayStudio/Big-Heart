using Entities.Item.Effect.Base;
using Entities.Item.Model;
using Services;
using UnityEngine;

namespace Entities.Item.Effect.Assets
{
    [CreateAssetMenu(menuName = "Effects/Simple periodic positive effect")]
    public class SimplePositiveEffectAsset : BaseEffectAsset, IPeriodicEffect
    {
        [Min(0)] public float value = 1f;
        [Min(0.1f)] public float interval = 1f;
        [Min(0)] public float duration = 0f; // 0 = бесконечно

        IDateProgressService _progress;
        IDateProgressService Progress => _progress ??= ServiceRegistry.Resolve<IDateProgressService>();

        public float IntervalSec => interval;
        public float DurationSec => duration;
        public void Tick() => Progress.AddPositive(value);

        public override IEffect BuildRuntime(ItemModel owner)
        {
            // эффект статический по параметрам — можно вернуть клон самого ассета
            return Instantiate(this);
        }
    }
}
