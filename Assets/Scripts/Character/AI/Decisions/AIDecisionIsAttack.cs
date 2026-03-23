using System.Collections;
using System.Collections.Generic;
using Larje.Character.AI;
using UnityEngine;

public class AIDecisionIsAttack : AIDecision
{
    private CharacterAttack _characterAttack;

    protected override void OnInitialized()
    {
        _characterAttack = Brain.gameObject.GetComponent<CharacterAttack>();
    }

    public override bool Decide()
    {
        if (_characterAttack != null)
        {
            return _characterAttack.IsAttacking; 
        }
        return false;
    }
}
