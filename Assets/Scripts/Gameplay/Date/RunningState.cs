using Events.Gameplay;
using Services;
using Services.Inventory;
using UnityEngine;

namespace Gameplay
{
    public class RunningState : IDateState
    {
        private readonly ICoroutineService _coroutineService;
        private readonly IEventService _eventService;
        private readonly IDateProgressService _dateProgressService;

        private const float NegativePerSec = 1.0f; // TODO: from config

        private uint _tickId;

        public RunningState()
        {
            _coroutineService = ServiceRegistry.Resolve<ICoroutineService>();
            _eventService = ServiceRegistry.Resolve<IEventService>();
            _dateProgressService = ServiceRegistry.Resolve<IDateProgressService>();
        }

        public void Enter()
        {
            _tickId = _coroutineService.RunRepeatingCoroutine(OnSecondTick, 1f);
        }

        public void Exit()
        {
            _eventService.Publish(new DateEndedEvent());
        }

        private void OnSecondTick()
        {
            _dateProgressService.AddNegative(NegativePerSec);
        }
    }
}