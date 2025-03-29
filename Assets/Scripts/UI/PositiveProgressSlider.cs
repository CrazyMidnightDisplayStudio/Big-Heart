using Services;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PositiveProgressSlider : MonoBehaviour
    {
        private Slider _slider;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.value = 0;
        }

        private void OnEnable()
        {
            EventService.Instance.OnDatePositiveProgressUpdate += _slider.SetValueWithoutNotify;
        }

        private void OnDisable()
        {
            EventService.Instance.OnDatePositiveProgressUpdate -= _slider.SetValueWithoutNotify;
        }
    }
}