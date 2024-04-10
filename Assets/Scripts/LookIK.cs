using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Tools.TopDownEngine;
using UnityEngine;

public class LookIK : MonoBehaviour
{
    private Animator _animator;
    private MoveBasedCharacterOrientation3D _orientation;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _orientation = GetComponentInParent<MoveBasedCharacterOrientation3D>();
    }
    
    private void OnAnimatorIK(int layerIndex)
    {
        if (_animator)
        {
            if (_orientation.forceLookTarget == null)
            {
                _animator.SetLookAtWeight(0f);
            }
            else
            {
                _animator.SetLookAtWeight(1f);
                _animator.SetLookAtPosition(_orientation.forceLookTarget.position);   
            }
        }   
    }
}
