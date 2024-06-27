using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInteractionTrigger : MonoBehaviour
{
    [SerializeField] private LayerMask mask;
    [SerializeField] private float duration = 1f;
    [Space]
    [SerializeField] private UnityEvent onInteract;
    
    private float _timer;
    private Collider _other;
    
    private void Update()
    {
        if (_other != null)
        {
            _timer += Time.deltaTime;
            if (_timer >= duration)
            {
                onInteract?.Invoke();
                _other = null;
            }
        }
        else
        {
            _timer = 0f;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (_other == null)
        {
            if (mask.HasLayer(other.gameObject.layer))
            {
                _other = other;
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (_other != null && _other == other)
        {
            _other = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
