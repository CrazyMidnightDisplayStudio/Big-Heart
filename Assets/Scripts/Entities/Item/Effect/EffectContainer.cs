using System;
using System.Collections.Generic;
using Events.Gameplay; // твои DateStarted/Ended/... события
using Services;

namespace Entities.Item.Effect
{
    public sealed class EffectContainer : IDisposable
    {
        private bool _enable;
        private readonly List<IEffect> _effects = new();
        private readonly List<uint> _periodicIds = new();

        private readonly IEventService _eventService;
        private readonly ICoroutineService _coroutineService;

        private readonly IDisposable[] _subscriptions;

        public EffectContainer(IEnumerable<IEffect> effects, IEventService eventService, ICoroutineService coroutineService)
        {
            _eventService = eventService;
            _coroutineService = coroutineService;

            if (effects != null) _effects.AddRange(effects);

            _subscriptions = new IDisposable[]
            {
                eventService.Subscribe<DateStartedEvent>(_ => { OnDateStart(); }),
                eventService.Subscribe<DateEndedEvent>(_ => { OnDateEnd(); }),
            };
        }

        void OnEquip()
        {
            _enable = true;
        }

        void OnUnequip()
        {
            _enable = false;
        }
        void OnDateStart()
        {
            if (!_enable) return;
            foreach (var e in _effects) (e as IOnDateStart)?.OnDateStart();
            StartPeriodic();
        }
        void OnDateEnd()
        {
            if (!_enable) return;
            StopPeriodic();
            foreach (var e in _effects) (e as IOnDateEnd)?.OnDateEnd();
        }

        void StartPeriodic()
        {
            if (!_enable) return;
            foreach (var e in _effects)
                if (e is IPeriodicEffect p)
                    _periodicIds.Add(_coroutineService.RunRepeatingCoroutine(p.Tick, p.IntervalSec, p.DurationSec));
        }
        void StopPeriodic()
        {
            if (_periodicIds.Count == 0) return;
            _coroutineService.Stop(_periodicIds);
            _periodicIds.Clear();
        }

        public void Dispose()
        {
            StopPeriodic();
            foreach (var s in _subscriptions) s?.Dispose();
        }
    }
}
