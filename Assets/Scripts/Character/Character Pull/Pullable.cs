using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pullable : MonoBehaviour
{
    [SerializeField] private List<Transform> attachPoints;
    
    private Transform _attachPoint;
    private Transform _attachTarget;
    private Rigidbody _rb;
    
    public Transform AttachPoint => _attachPoint;
    public Rigidbody Rigidbody => _rb;

    public event Action EventForceDetach; 

    public Transform NearestAttachToPoint(Vector3 point)
    {
        return attachPoints.OrderBy(x => Vector3.Distance(x.position, point)).First();
    }

    public void Attach(Transform attachTarget, Transform attachPoint)
    {
        _attachTarget = attachTarget;
        _attachPoint = attachPoint;
    }
    
    public void Detach()
    {
        _attachTarget = null;
        _attachPoint = null;
    }

    public void UpdatePosition()
    {
        if (_attachTarget != null)
        {
            Vector3 direction = _attachTarget.position - _attachPoint.position;
            //direction -= direction.normalized * _attachDistance;
            float speed = direction.magnitude / Time.fixedDeltaTime;
            _rb.velocity = _rb.velocity.Y();
            _rb.angularVelocity = Vector3.zero;
            _rb.AddForceAtPosition(direction.normalized * speed, _attachPoint.position, ForceMode.VelocityChange);
        }
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        EventForceDetach?.Invoke();
    }
    
    private void OnDestroy()
    {
        EventForceDetach?.Invoke();
    }
}
