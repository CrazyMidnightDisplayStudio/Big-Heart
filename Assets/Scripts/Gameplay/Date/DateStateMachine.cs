using System;
using Events.Gameplay;
using Services;

namespace Gameplay
{
    public class DateStateMachine
    {
        private readonly IDateState _waiting, _running, _paused, _resumed;
        private IDateState _currentState;

        private readonly IEventService _eventService;
        private readonly ICoroutineService _coroutineService;
        
        private readonly IDisposable _subStart;

        public DateStateMachine()
        {
            _eventService = ServiceRegistry.Resolve<IEventService>();
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
            ChangeTo(_running);
        }

        private void Pause()
        {
            _eventService.Publish(new DatePausedEvent());
        }

        private void Resume()
        {
            _eventService.Publish(new DateResumedEvent());
        }

        private void End()
        {
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