using Services;
//using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Progress : MonoBehaviour
{
    [SerializeField] private float currentValue = 0f;
    [SerializeField] private float maxValue = 100f;
    [SerializeField] private Slider progressBar;

    private void Awake()
    {
        progressBar = GetComponent<Slider>();
        progressBar.maxValue = maxValue;
        progressBar.value = currentValue;
        progressBar.wholeNumbers = false;
    }

    private void OnEnable()
    {
        Services.EventService.OnDateProgressChanged += FillProgress;
    }

    private void OnDisable()
    {
        Services.EventService.OnDateProgressChanged -= FillProgress;
    }

    private void FillProgress(float amount)
    {
        progressBar.value += amount;
        if (progressBar.value >= maxValue)
        {
            EventService.DateEnd();
        }
        else if (progressBar.value <= 0)
        {
            progressBar.value = 0;
        }
    }
}
