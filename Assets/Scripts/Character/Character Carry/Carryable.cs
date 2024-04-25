using System;
using DG.Tweening;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Carryable : MonoBehaviour
{
    [SerializeField] private Transform topPoint;
    [Space] 
    [SerializeField] private float maxRotate = 5f;
    [SerializeField] private float dropForce;

    [Header("Anchoring Animation")] 
    [SerializeField] private float anchoringBaseSpeed;
    [SerializeField] private float anchoringAcceleration;
    [SerializeField] private AnimationCurve anchoringTrajectory;

    private bool _blockTaking;
    private bool _anchored;
    private float _currentSpeed;

    private Vector3 _takePosition;

    private Rigidbody _rb;
    private Collider _collider;
    private Transform _attachPoint;

    public bool CanBeTaken => _attachPoint == null && !_blockTaking;
    public Rigidbody Rigidbody => _rb;
    public Transform TopPoint => topPoint;

    public event Action<Carryable> EventDisabled;

    public void Take(Transform attachPoint)
    {
        _attachPoint = attachPoint;
        
        _anchored = false;
        _rb.isKinematic = true;
        _collider.enabled = false;
        
        _currentSpeed = anchoringBaseSpeed;
        _takePosition = transform.position;
        
        PlayAnchoringAnimation();
    }

    public void Drop(bool blockTaking = false)
    {
        this.DOKill();

        _blockTaking = blockTaking;
        _attachPoint = null;
        _rb.isKinematic = false;
        _collider.enabled = true;

        Vector3 force = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)) * Vector3.forward;
        force.y = 1f;
        force *= dropForce;
        _rb.AddForce(force, ForceMode.VelocityChange);
    }

    public void UpdatePosition(float deltaTime, float movementSpeed)
    {
        TryUpdatePosition(movementSpeed);
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
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
            transform.position = Vector3.Lerp(_takePosition, _attachPoint.position, x);
        }, 1f, 1f)
            .OnComplete(() => _anchored = true)
            .SetTarget(this);
    }

    private void TryUpdatePosition(float movementSpeed)
    {
        if (_attachPoint != null && _anchored)
        {
            float rotate = maxRotate * movementSpeed;

            transform.position = _attachPoint.position;
            transform.rotation = _attachPoint.rotation;

            transform.rotation *= Quaternion.Euler(Vector3.right * -rotate);
        }
    }
}
