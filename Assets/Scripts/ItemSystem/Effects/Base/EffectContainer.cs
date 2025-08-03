using System;
using System.Collections.Generic;
using Events.Gameplay;
using Services;

namespace ItemSystem.Effects
{
    public sealed class EffectContainer : IDisposable
    {
        private readonly List<IEffect> _effects = new();
        private readonly List<uint> _periodicCoroutineIds = new();
        private readonly IDisposable[] _subscriptions;

        private readonly ICoroutineService _coroutineService;

        public EffectContainer(IEnumerable<IEffect> effects)
        {
            var eventService = ServiceRegistry.Resolve<IEventService>();
            if (eventService == null)
            {
                throw new ArgumentNullException(nameof(eventService));
            }

            _coroutineService = ServiceRegistry.Resolve<ICoroutineService>();
            if (_coroutineService == null)
            {
                throw new ArgumentNullException(nameof(_coroutineService));
            }

            _effects.AddRange(effects);

            _subscriptions = new IDisposable[]
            {
                eventService.Subscribe<DateStartedEvent>(_ =>
                {
                    foreach (var effect in _effects)
                    {
                        if (effect is IOnDateStart onDateStartEffect)
                        {
                            onDateStartEffect.OnDateStart();
                        }
                    }
                    StartPeriodicEffects();
                }),
                eventService.Subscribe<DateEndedEvent>(_ =>
                {
                    foreach (var effect in _effects)
                    {
                        if (effect is IOnDateEnd onDateEndEffect)
                        {
                            onDateEndEffect.OnDateEnd();
                        }
                    }
                    StopPeriodicEffects();
                }),
            };
        }

        public void Dispose()
        {
            foreach (var e in _subscriptions) e.Dispose();
            StopPeriodicEffects();
        }

        private void StartPeriodicEffects()
        {
            foreach (var e in _effects)
            {
                if (e is IPeriodicEffect periodicEffect)
                {
                    var id = _coroutineService.RunRepeatingCoroutine(periodicEffect.Tick, periodicEffect.IntervalSec,
                        periodicEffect.DurationSec);
                    _periodicCoroutineIds.Add(id);
                }
            }
        }

        private void StopPeriodicEffects()
        {
            _coroutineService.Stop(_periodicCoroutineIds);
            _periodicCoroutineIds.Clear();
        }
    }
}