using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cargo : MonoBehaviour
{
    [SerializeField] private List<Transform> attachPoints;
    [SerializeField] private float rotationSpeed;

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

    private void OnDrawGizmos()
    {
        if (_rb != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(_rb.worldCenterOfMass, 0.4f);
        }
    }

    private void FixedUpdate()
    {
        if (_attachTarget != null)
        {
            Vector3 direction = _attachTarget.position - _attachPoint.position;
            direction -= direction.normalized * _attachDistance;

            float speed = direction.magnitude / Time.fixedDeltaTime;

            _rb.velocity = _rb.velocity.Y();
            _rb.angularVelocity = Vector3.zero;
            _rb.AddForceAtPosition(direction.normalized * speed, _attachPoint.position, ForceMode.VelocityChange);

            Vector3 balancePoint = (_attachPoint.position - _attachPoint.forward * 10).XZ();
            Vector3 projNormal = (_attachPoint.position - _attachTarget.position).normalized.XZ();
            Vector3 projection = _attachPoint.position + Vector3.Project(-_attachPoint.forward * 10, projNormal).XZ();
            Vector3 balanceDirection = projection - balancePoint;
            float balanceSpeed = balanceDirection.magnitude * rotationSpeed;
            _rb.AddForceAtPosition(balanceDirection.normalized * balanceSpeed, balancePoint, ForceMode.VelocityChange);
            
            Debug.DrawLine(_attachPoint.position, balancePoint, Color.green);
            Debug.DrawLine(projection, balancePoint, Color.red);
            Debug.DrawLine(_attachTarget.position, _attachTarget.position + projNormal * 15f, Color.blue);
        }
    }
}
