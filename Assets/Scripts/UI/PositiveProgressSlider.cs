using System;
using CMD.Services;
using Events.UI;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Slider))]
    public class PositiveProgressSlider : MonoBehaviour
    {
        private Slider _slider;
        private IDisposable _subscription;
        private IEventBusService _eventService;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.value = 0;
            _eventService = ServiceRegistry.Resolve<IEventBusService>();
        }

        private void OnEnable()
        {
            _subscription =
                _eventService.Subscribe<PositiveSliderUpdateEvent>(e => _slider.SetValueWithoutNotify(e.Value));
        }

        private void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}
