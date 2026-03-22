using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Character;
using UnityEngine;

public class LookIK : MonoBehaviour
{
    [SerializeField] private float defaultLookHeight = 1.5f;
    [SerializeField] private float weightChangeSharpness = 2f;

    private float _weight = 0f;
    private Animator _animator;
    private CharacterOrientation _orientation;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _orientation = GetComponentInParent<CharacterOrientation>();
    }
    
    private void Update()
    {
        if (_orientation)
        {
            // _weight = Mathf.Lerp(_weight, _orientation.PermitLook ? 1f : 0f, Time.deltaTime * weightChangeSharpness);
        }
    }
    
    private void OnAnimatorIK(int layerIndex)
    {
        if (_animator && _orientation)
        {
            _animator.SetLookAtWeight(_weight);
            Vector3 lookPosition = transform.position;
            lookPosition += (Vector3.up * defaultLookHeight) + (Vector3.forward/*_orientation.LookDirection*/ * 10f);
            _animator.SetLookAtPosition(lookPosition);   
        }   
    }
}
