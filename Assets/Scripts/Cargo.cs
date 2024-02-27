using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cargo : MonoBehaviour
{
    [SerializeField] private List<Transform> attachPoints;
    
    private Transform _attachPoint;
    private Rigidbody _rb;
    
    public Transform AttachPoint => _attachPoint;
    public Rigidbody Rigidbody => _rb;

    public Transform NearestAttachToPoint(Vector3 point)
    {
        return attachPoints.OrderBy(x => Vector3.Distance(x.position, point)).First();
    }

    public void Attach(Transform attachPoint)
    {
        //_rb.isKinematic = false;
        _attachPoint = attachPoint;
    }
    
    public void Detach()
    {
        //_rb.isKinematic = true;
        _attachPoint = null;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        //_rb.isKinematic = true;
    }
}
