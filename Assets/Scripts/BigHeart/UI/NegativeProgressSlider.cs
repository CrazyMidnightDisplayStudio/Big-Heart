using System;
using CMD.Core;
using CMD.Services;
using Events.UI;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Slider))]
    public class NegativeProgressSlider : MonoBehaviour
    {
        private Slider _slider;
        private IDisposable _subscription;
        private IEventBusService _eventService;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.value = 0;
            _eventService = ServiceRegistry.Get<IEventBusService>();
        }

        private void OnEnable()
        {
            _subscription =
                _eventService.Subscribe<NegativeSliderUpdateEvent>(e => _slider.SetValueWithoutNotify(e.Value));
        }

        private void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}
