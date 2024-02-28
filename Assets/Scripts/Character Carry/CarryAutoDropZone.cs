using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryAutoDropZone : MonoBehaviour
{
    [SerializeField] private LayerMask mask;
    [SerializeField] private float dropDelay;

    private float _time;
    private List<CharacterCarry3D> _carries;

    private void Start()
    {
        _carries = new List<CharacterCarry3D>();
    }

    private void Update()
    {
        _time += Time.deltaTime;
        if (_time >= dropDelay)
        {
            _time = 0f;
            _carries.ForEach(x => x.TryDrop());
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
}
