using CMD.Services;
using Services;

namespace Gameplay
{
    public class WaitingState : IDateState
    {
        private readonly IEventBusService _eventService;

        public WaitingState()
        {
            _eventService = ServiceRegistry.Resolve<IEventBusService>();
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
