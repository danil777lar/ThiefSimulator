using System;
using DG.Tweening;
using UnityEngine;

using Random = UnityEngine.Random;

public class Carryable : MonoBehaviour
{
    [SerializeField] private float weight;
    [Space]
    [SerializeField] private CarryableConfig config;
    [SerializeField] private Transform topPoint;

    private bool _blockTaking;
    private bool _anchored;
    private float _currentRotation;
    private float _currentForce;

    private Vector3 _takePosition;

    private Rigidbody _rb;
    private Collider _collider;
    private Transform _attachPoint;

    public bool CanBeTaken => _attachPoint == null && !_blockTaking;
    public float Weight => weight;
    public Rigidbody Rigidbody => _rb;
    public Transform TopPoint => topPoint;

    public event Action<Carryable> EventDisabled;

    public void Take(Transform attachPoint)
    {
        _attachPoint = attachPoint;
        
        _anchored = false;
        _rb.isKinematic = true;
        _collider.enabled = false;
        _takePosition = transform.position;
        
        _currentRotation = 0f;
        _currentForce = 0f;
        
        PlayAnchoringAnimation();
    }

    public void Drop(bool blockTaking = false)
    {
        this.DOKill();

        _blockTaking = blockTaking;
        _attachPoint = null;
        _rb.isKinematic = false;
        _collider.enabled = true;
        
        _currentRotation = 0f;
        _currentForce = 0f;

        Vector3 force = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)) * Vector3.forward;
        force.y = 1f;
        force *= config.DropForce;
        _rb.AddForce(force, ForceMode.VelocityChange);
    }

    public void UpdatePosition(float deltaTime, float speedPercent)
    {
        TryUpdatePosition(deltaTime, speedPercent);
    }

    public void SetInteractable(bool value)
    {
        _blockTaking = !value;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponentInChildren<Collider>();
    }


    private void OnDisable()
    {
        EventDisabled?.Invoke(this);
    }

    private void OnDestroy()
    {
        EventDisabled?.Invoke(this);
    }

    private void PlayAnchoringAnimation()
    {
        DOTween.To(() => 0f, (x) =>
                {
                    transform.position = EvaluateTrajectory(x);
                    transform.rotation = Quaternion.Lerp(transform.rotation, _attachPoint.rotation, x);
                }, 
                1f, config.AnchoringDuration)
            .OnComplete(() => _anchored = true)
            .SetEase(Ease.OutBounce)
            .SetTarget(this);
    }

    private void TryUpdatePosition(float deltaTime, float speedPercent)
    {
        if (_attachPoint != null && _anchored)
        {
            float forceDelta = config.MaxRotate * speedPercent * config.ForceMultiplier * deltaTime; 
            forceDelta += config.SpringForce * (0f - _currentRotation) * deltaTime;

            bool canAddForce = false;
            canAddForce |= forceDelta < 0f && _currentRotation > -config.MaxRotate;
            canAddForce |= forceDelta > 0f && _currentRotation < config.MaxRotate;
                
            if (canAddForce)
            {
                _currentForce += forceDelta;
            }
            _currentForce *= 1f - config.SpringDrag * deltaTime;
            
            _currentRotation += _currentForce * deltaTime;
            _currentRotation = Mathf.Clamp(_currentRotation, -config.MaxRotate, config.MaxRotate);
            
            transform.position = _attachPoint.position;
            transform.rotation = _attachPoint.rotation;

            transform.rotation *= Quaternion.Euler(Vector3.right * -_currentRotation);
        }
    }
    
    private Vector3 EvaluateTrajectory(float time)
    {
        Vector3 a = _takePosition;
        Vector3 b = _attachPoint.position + Vector3.up * config.AnchoringTrajectoryHeight;
        Vector3 c = _attachPoint.position;
        Vector3 ab = Vector3.Lerp(a, b, time);
        Vector3 bc = Vector3.Lerp(b, c, time);
        return Vector3.Lerp(ab, bc, time);
    }
}
