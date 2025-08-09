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

        public float IntervalSec => interval;
        public float DurationSec => duration;

        private bool EnsureService()
        {
            if (_progress != null)
            {
                return true;
            }
            // В Play Mode сервис обязан быть; в Editor – просто молчим.
            if (!Application.isPlaying)
            {
                return false;
            }
            _progress = Services.ServiceRegistry.Resolve<IDateProgressService>();
            return _progress != null;
        }

        public void Tick()
        {
            EnsureService();
            _progress.AddPositive(value);
        }

        public override IEffect BuildRuntime(ItemModel owner)
        {
            return Instantiate(this); // клонируем и возвращаем IEffect
        }
    }
}
