using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanAnimation : MonoBehaviour
{
    [SerializeField] private Transform model;
    [Header("Center Of Mass")] 
    [SerializeField] private Vector3 offset = Vector3.up;
    [SerializeField] private float maxDistance = 1f;
    [Space]
    [SerializeField] private float force = 10f;
    [SerializeField] private float damping = 10f;

    private Vector3 _velocity;
    private Vector3 _centerOfMass;
    private VanMovement _movement;
    
    private Vector3 DefaultPosition => model.transform.position + offset;

    private void Start()
    {
        _movement = GetComponent<VanMovement>();
        _centerOfMass = DefaultPosition;
    }

    private void FixedUpdate()
    {
        Vector3 direction = (_centerOfMass - DefaultPosition);
        _centerOfMass = DefaultPosition + direction.normalized * Mathf.Min(maxDistance, direction.magnitude);
        
        _centerOfMass = Vector3.Lerp(_centerOfMass, DefaultPosition, Time.deltaTime * 10f);
        model.LookAt(_centerOfMass, -transform.forward);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(DefaultPosition, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_centerOfMass, 0.1f);
    }
}
