using System.Collections;
using System.Collections.Generic;
using Larje.Core.Tools.TopDownEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class AIActionSetMovementSpeedMultiplier : AIAction
{
    [SerializeField] private float multiplier = 1f;

    private bool _isInState;
    
    public override void Initialization()
    {
        base.Initialization();
        CoreCharacterMovement movement = _brain.Owner.GetComponent<CoreCharacterMovement>();
        movement.TryAddSpeedMultiplier(GetSpeedMultiplier);
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        _isInState = true;
    }
    
    public override void OnExitState()
    {
        base.OnExitState();
        _isInState = false;
    }

    public override void PerformAction()
    {
        
    }

    private float GetSpeedMultiplier()
    {
        return _isInState && gameObject.activeInHierarchy ? multiplier : 1f;
    }
}
