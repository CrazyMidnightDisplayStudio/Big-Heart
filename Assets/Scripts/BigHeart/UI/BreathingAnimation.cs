using UnityEngine;
using DG.Tweening;

public class BreathingAnimation : MonoBehaviour
{
    [SerializeField] private float breathInDuration = 1.5f;
    [SerializeField] private float breathOutDuration = 2f;
    [SerializeField] private float breathAmplitude = 0.1f;

    private Transform _targetTransform;
    private Sequence _breathSequence;
    private Vector3 _initialScale;

    private void Start()
    {
        _targetTransform = GetComponent<Transform>();
        _initialScale = _targetTransform.localScale;
        CreateBreathSequence();
    }

    private void OnDestroy()
    {
        _breathSequence.Kill();
    }

    private void CreateBreathSequence()
    {
        _breathSequence = DOTween.Sequence();

        float scaleMultiplier = 1 + breathAmplitude;
        Vector3 maxScale = new Vector3(_initialScale.x * scaleMultiplier, _initialScale.y * scaleMultiplier, _initialScale.z);

        _breathSequence.Append(_targetTransform.DOScale(maxScale, breathInDuration).SetEase(Ease.InOutSine));

        _breathSequence.Append(_targetTransform.DOScale(_initialScale, breathOutDuration).SetEase(Ease.InOutSine));

        _breathSequence.SetLoops(-1, LoopType.Yoyo);
    }
}
