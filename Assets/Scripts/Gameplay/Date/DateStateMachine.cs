using System;
using CMD.Services;
using Events.Gameplay;
using Services;
using UnityEngine;

namespace Gameplay
{
    public class DateStateMachine
    {
        private readonly IDateState _waiting, _running, _paused, _resumed;
        private IDateState _currentState;

        private readonly IEventBusService _eventService;
        private readonly ICoroutineService _coroutineService;

        private readonly IDisposable _subStart;

        public DateStateMachine()
        {
            _eventService = ServiceRegistry.Resolve<IEventBusService>();
            _coroutineService = ServiceRegistry.Resolve<ICoroutineService>();

            _waiting = new WaitingState();
            _running = new RunningState();

            _subStart = _eventService.Subscribe<DateStartedEvent>(_ => StartDate());
        }

        public void StartScene()
        {
            ChangeTo(_waiting);
        }

        public void StartDate()
        {
            Debug.Log("Date started");
            ChangeTo(_running);
        }

        private void Pause()
        {
            Debug.Log("Date paused");
            _eventService.Publish(new DatePausedEvent());
        }

        private void Resume()
        {
            Debug.Log("Date resumed");
            _eventService.Publish(new DateResumedEvent());
        }

        private void End()
        {
            Debug.Log("Date ended");
            _eventService.Publish(new DateEndedEvent());
        }

        void ChangeTo(IDateState next)
        {
            _currentState?.Exit();
            _currentState = next;
            _currentState.Enter();
        }
    }
}
