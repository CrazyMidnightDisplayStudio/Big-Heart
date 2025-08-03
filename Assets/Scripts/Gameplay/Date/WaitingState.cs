using Events.Gameplay;
using Services;
using UnityEngine;

namespace Gameplay
{
    public class WaitingState : IDateState
    {
        private readonly IEventService _eventService;

        public WaitingState()
        {
            _eventService = ServiceRegistry.Resolve<IEventService>();
        }
        
        public void Enter()
        {
            // TODO: показать - нажми старт
        }

        public void Exit()
        {
            // TODO: ?
        }
    }
}