using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class AIActionSetMovementSpeedMultiplier : AIAction
{
    [SerializeField] private float multiplier = 1f;
    
    private CharacterMovement _movement; 
    
    public override void Initialization()
    {
        base.Initialization();
        _movement = _brain.Owner.GetComponent<CharacterMovement>();
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        _movement.SetContextSpeedMultiplier(multiplier);
    }
    
    public override void OnExitState()
    {
        base.OnExitState();
        _movement.ResetContextSpeedMultiplier();
    }

    public override void PerformAction()
    {
        
    }
}
