using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryAutoDropZone : MonoBehaviour
{
    [SerializeField] private float dropDelay;
    [SerializeField] private float pullForce;
    [SerializeField] private LayerMask mask;

    private float _time;
    private List<CharacterCarry3D> _carries;
    private List<Carryable> _carryables;

    private void Start()
    {
        _carries = new List<CharacterCarry3D>();
        _carryables = new List<Carryable>();
    }

    private void Update()
    {
        _time += Time.deltaTime;
        if (_time >= dropDelay)
        {
            _time = 0f;
            _carries.ForEach(x => AddCarryable(x.TryDrop(true)));
        }
    }
    
    private void FixedUpdate()
    {
        foreach (Carryable carryable in _carryables)
        {
            Vector3 force = (transform.position - carryable.transform.position).normalized * pullForce; 
            carryable.Rigidbody.AddForce(force, ForceMode.Acceleration);
        }
    }
        
    private void OnTriggerEnter(Collider other)
    {
        if (mask == (mask | (1 << other.gameObject.layer)))
        {
            CharacterCarry3D carry = other.GetComponentInParent<CharacterCarry3D>();
            if (carry != null && !_carries.Contains(carry))
            {
                _carries.Add(carry);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (mask == (mask | (1 << other.gameObject.layer)))
        {
            CharacterCarry3D carry = other.GetComponentInParent<CharacterCarry3D>();
            if (carry != null && _carries.Contains(carry))
            {
                _carries.Remove(carry);
            }   
        }
    }

    private void AddCarryable(Carryable carryable)
    {
        if (carryable != null && !_carryables.Contains(carryable))
        {
            carryable.EventDisabled += RemoveCarryable;
            _carryables.Add(carryable);
        }
    }
    
    private void RemoveCarryable(Carryable carryable)
    {
        if (carryable != null && _carryables.Contains(carryable))
        {
            carryable.EventDisabled -= RemoveCarryable;
            _carryables.Remove(carryable);
        }
    }
}
