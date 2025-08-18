using System;
using CMD.Services;
using Events.Gameplay;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class StartDateButton : MonoBehaviour
    {
        private Button _button;
        private IEventBusService _eventService;

        void Awake()
        {
            _button       = GetComponent<Button>();
            _button.interactable = true;
            _eventService = ServiceRegistry.Resolve<IEventBusService>();

            _button.onClick.AddListener(OnClick);
        }

        void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            _eventService.Publish(new DateStartedEvent());
            _button.interactable = false;
            _button.gameObject.SetActive(false);
        }
    }
}
