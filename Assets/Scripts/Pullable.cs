using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pullable : MonoBehaviour
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
        _attachPoint = attachPoint;
    }
    
    public void Detach()
    {
        _attachPoint = null;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
}
