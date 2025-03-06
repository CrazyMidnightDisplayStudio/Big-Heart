using DG.Tweening;
using UnityEngine;

public class ItemFill : MonoBehaviour
{
    private static readonly int FillRate = Shader.PropertyToID("_FillRate");
    
    [SerializeField] private float duration = 5f;
    [SerializeField] private float minValue = -0.25f;
    [SerializeField] private float maxValue = 0.25f;
    
    [SerializeField] private float rotationDuration = 1f;
    [SerializeField] private float rotationAngle = 30f;
    
    [SerializeField] private Material itemProgressMaterial;

    private float _fillRate;
    private float _timer;
    
    private Tween _tweenRotateShake;

    private void Update()
    {
        _timer += Time.deltaTime;

        _fillRate = Mathf.Lerp(minValue, maxValue, _timer / duration);

        itemProgressMaterial.SetFloat(FillRate, _fillRate);

        if (!(_fillRate >= maxValue)) return;
        
        _timer = 0f;
        OnCycleEnd();
    }

    private void OnCycleEnd()
    {
        if (_tweenRotateShake != null)
        {
            _tweenRotateShake.Restart();
            return;
        }
        
        _tweenRotateShake = transform.DORotate(new Vector3(0, 0, rotationAngle), rotationDuration / 2f).OnComplete(() =>
        {
            transform.DORotate(new Vector3(0, 0, -rotationAngle), rotationDuration / 2f).OnComplete(() =>
            {
                transform.DORotate(new Vector3(0, 0, 0), rotationDuration / 2f);
            });
        }).SetAutoKill(false);
    }
}

