using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cargo : MonoBehaviour
{
    [SerializeField] private List<Transform> attachPoints;

    private Transform _attachTarget;
    private Transform _attachPoint;
    private float _attachDistance;

    private Rigidbody _rb;

    public Transform NearestAttachToPoint(Vector3 point)
    {
        return attachPoints.OrderBy(x => Vector3.Distance(x.position, point)).First();
    }

    public void Attach(Transform attachTarget, Transform attachPoint, float distance)
    {
        _rb.isKinematic = false;
        
        _attachTarget = attachTarget;
        _attachPoint = attachPoint;
        _attachDistance = distance;
    }
    
    public void Detach()
    {
        _rb.isKinematic = true;
        
        _attachTarget = null;
        _attachPoint = null;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
    }

    private void FixedUpdate()
    {
        if (_attachTarget != null)
        {
            Vector3 direction = _attachTarget.position - _attachPoint.position;
            direction -= direction.normalized * _attachDistance;

            float speed = direction.magnitude / Time.fixedDeltaTime;

            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.AddForceAtPosition(direction.normalized * speed, _attachPoint.position, ForceMode.VelocityChange);
        }
    }
}
