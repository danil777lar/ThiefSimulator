using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class AIDecisionIsAttack : AIDecision
{
    private CharacterAttack _characterAttack;

    public override void Initialization()
    {
        base.Initialization();
        _characterAttack = _brain.gameObject.GetComponent<CharacterAttack>();
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
