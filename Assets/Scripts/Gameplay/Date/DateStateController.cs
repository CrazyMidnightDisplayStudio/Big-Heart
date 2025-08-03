using UnityEngine;

namespace Gameplay
{
    public class DateStateController : MonoBehaviour
    {
        private DateStateMachine _dateStateMachine;

        private void Awake()
        {
            // В данный момент используем один контейнер для регистрации сервисов,
            // который инициализируется в Scripts/Base/GameBootstrap.cs
            // TODO: register child DI container
            
            _dateStateMachine = new DateStateMachine();
        }

        private void Start()
        {
            _dateStateMachine.StartScene();
        }
    }
}