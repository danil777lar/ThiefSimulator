using System.Collections;
using System.Collections.Generic;
using Larje.Character.Abilities;
using Larje.Character.AI;
using UnityEngine;

public class AIActionSetMovementSpeedMultiplier : AIAction
{
    [SerializeField] private float multiplier = 1f;

    private bool _isInState;
    
    public override void PerformAction()
    {
        
    }

    protected override void OnInitialized()
    {
        CharacterWalk movement = Brain.Owner.GetComponent<CharacterWalk>();
        movement.WalkMultiplier.AddValue(GetSpeedMultiplier);
    }

    protected override void OnEnterState()
    {
        _isInState = true;
    }
    
    protected override void OnExitState()
    {
        _isInState = false;
    }

    private float GetSpeedMultiplier()
    {
        return _isInState && gameObject.activeInHierarchy ? multiplier : 1f;
    }
}
