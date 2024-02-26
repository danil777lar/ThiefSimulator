using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class MoveCharacterOrientation3D : CharacterAbility
{
    [SerializeField] private float minRotationSpeed;
    [SerializeField] private float maxRotationSpeed;
    [Space]
    [SerializeField] private Transform model;
    
    private Vector3 _lastPosition;
    private Vector3 _currentDirection;
    private BlendedCharacterMovement _blendedMovement;

    protected override void Initialization()
    {
        base.Initialization();
        _blendedMovement = _character.FindAbility<BlendedCharacterMovement>();
    }

    public override void ProcessAbility()
    {
        base.ProcessAbility();

        if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
        {
            return;
        }

        if (model == null)
        {
            return;
        }

        if (!AbilityAuthorized)
        {
            return;
        }

        CatchDirection();
        Rotate();
    }

    private void CatchDirection()
    {
        _currentDirection = (transform.position - _lastPosition).normalized;
        _lastPosition = transform.position;
    }
    
    private void Rotate()
    {
        if (_currentDirection != Vector3.zero)
        {
            float rotationSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, _blendedMovement.ActualSpeedPercent);
            Quaternion rotation = Quaternion.LookRotation(_currentDirection);
            model.rotation = Quaternion.RotateTowards(model.rotation, rotation, rotationSpeed);
        }
    }
}