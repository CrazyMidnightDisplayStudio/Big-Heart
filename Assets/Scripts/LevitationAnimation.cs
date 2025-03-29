using ItemSystem;
using UnityEngine;

public class SpriteLevitation : MonoBehaviour
{
    [SerializeField] private float levitationHeight = 0.5f;
    [SerializeField] private float levitationDistanceX = 0.2f;
    [SerializeField] private float levitationSpeedX = 2.0f;
    [SerializeField] private float levitationSpeedY = 1.0f;
    [SerializeField] private float randomOffsetMagnitude = 0.05f;
    [SerializeField] private float lerpSpeed = 5.0f;

    private Vector3 _initialPosition;
    private Vector3 _targetPosition;

    private System.Random _random;
    private void Start()
    {
        InitRandom();
        UpdatePositionMove();
    }
    private void InitRandom()
    {
        int hash = gameObject.GetHashCode();
        _random = new System.Random(Mathf.Abs(hash));
    }
    private void UpdatePositionMove()
    {
        _initialPosition = transform.localPosition;
        _targetPosition = CalculateNewTargetPosition();
    }
    private void Update()
    {

        _targetPosition = CalculateNewTargetPosition();
        transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPosition, Time.deltaTime * lerpSpeed);
    }

    private Vector3 CalculateNewTargetPosition()
    {
        float offsetX = Mathf.Sin(Time.time * levitationSpeedX) * levitationDistanceX;
        float offsetY = Mathf.Cos(Time.time * levitationSpeedY) * levitationHeight;
        
        float randomOffsetX = GetRandmValue(randomOffsetMagnitude, randomOffsetMagnitude * 2f);
        float randomOffsetY = GetRandmValue(randomOffsetMagnitude, randomOffsetMagnitude * 2f);
        return _initialPosition + new Vector3(offsetX + randomOffsetX, offsetY + randomOffsetY, 0f);
    }
    private float GetRandmValue(float min, float max)
    {
        return (float)(_random.NextDouble() * (max - min) + min);
    }
}
