using Services;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NegativeProgressSlider : MonoBehaviour
    {
        private Slider _slider;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.value = 0;
        }

        private void OnEnable()
        {
            EventService.Instance.OnDateNegativeProgressUpdate += _slider.SetValueWithoutNotify;
        }

        private void OnDisable()
        {
            EventService.Instance.OnDateNegativeProgressUpdate -= _slider.SetValueWithoutNotify;
        }
    }
}
