using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class VanAnimation : MonoBehaviour
{
    [SerializeField] private Transform model;

    [Header("Center Of Mass")] 
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance = 0.75f;
    [SerializeField] private Vector3 offset = Vector3.up;
    [Space]
    [SerializeField] private float force = 10f;
    [SerializeField] private float drag = 1f;

    private Vector3 _velocity;
    private Vector3 _centerOfMass;
    
    private Vector3 DefaultPosition => model.transform.position + offset;

    private void Start()
    {
        _centerOfMass = DefaultPosition;
    }

    private void FixedUpdate()
    {
        Vector3 direction = DefaultPosition - _centerOfMass;
        _centerOfMass = DefaultPosition - (direction.normalized * Mathf.Min(direction.magnitude, maxDistance)); 
        if (Vector3.Distance(DefaultPosition, _centerOfMass) > minDistance)
        {
            _velocity += direction.normalized * (force * Time.fixedDeltaTime);
        }
        _velocity *= 1f - Time.fixedDeltaTime * drag;
        _centerOfMass += _velocity * Time.fixedDeltaTime;
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
