using Services;
using UnityEngine;

namespace ItemSystem.Effects.Assets
{
    [CreateAssetMenu(menuName = "Effects/Simple periodic positive effect")]
    public class SimplePositiveBaseEffect : BaseEffectAsset, IPeriodicEffect
    {
        [Min(0)] public float value = 1;
        [Min(0.1f)] public float interval = 1;
        [Min(0)] public float duration = 0;

        IDateProgressService _progress;

        void OnEnable() =>
            _progress = ServiceRegistry.Resolve<IDateProgressService>();

        public float IntervalSec => interval;
        public float DurationSec => duration;

        public void Tick() => _progress.AddPositive(value);

        public override IEffect BuildRuntime(ItemModel owner)
        {
            return Instantiate(this); // клонируем и возвращаем IEffect
        }
    }
}
