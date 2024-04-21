using System;
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
    
    private bool _blockTaking;
    private float _anchoringValue;
    private Rigidbody _rb;
    private Collider _collider;
    private Transform _attachPoint;

    public bool CanBeTaken => _attachPoint == null && !_blockTaking;
    public Rigidbody Rigidbody => _rb;
    public Transform TopPoint => topPoint;

    public event Action<Carryable> EventDisabled;

    public void Take(Transform attachPoint, float anchoringValue)
    {
        _anchoringValue = anchoringValue;
        _attachPoint = attachPoint;
        _rb.isKinematic = true;
        _collider.enabled = false;
    }
    
    public void Drop(bool blockTaking = false)
    {
        _blockTaking = blockTaking;
        _attachPoint = null;
        _rb.isKinematic = false;
        _collider.enabled = true;

        Vector3 force = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)) * Vector3.forward;
        force.y = 1f;
        force *= dropForce;
        _rb.AddForce(force, ForceMode.VelocityChange);
    }

    public void UpdatePosition(float movementSpeed)
    {
        if (_attachPoint != null)
        {
            float rotate = maxRotate * movementSpeed;
            
            transform.position = _attachPoint.position;
            transform.rotation = _attachPoint.rotation;

            transform.rotation *= Quaternion.Euler(Vector3.right * -rotate);
        }
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
}
