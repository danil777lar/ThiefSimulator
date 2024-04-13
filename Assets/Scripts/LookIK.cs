using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Tools.TopDownEngine;
using UnityEngine;

public class LookIK : MonoBehaviour
{
    [SerializeField] private float defaultLookHeight = 1.5f;
    
    private Animator _animator;
    private CoreCharacterOrientation3D _orientation;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _orientation = GetComponentInParent<CoreCharacterOrientation3D>();
    }
    
    private void OnAnimatorIK(int layerIndex)
    {
        if (_animator)
        {
            _animator.SetLookAtWeight(1f);
            
            Vector3 lookPosition = transform.position + 
                                   (Vector3.up * defaultLookHeight) + (_orientation.LookDirection * 10f);
            
            _animator.SetLookAtPosition(lookPosition);   
        }   
    }
}
