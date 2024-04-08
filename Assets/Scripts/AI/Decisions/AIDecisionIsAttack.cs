using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class AIDecisionIsAttack : AIDecision
{
    private CharacterAttention _characterAttention;

    public override void Initialization()
    {
        base.Initialization();
        _characterAttention = _brain.Owner.GetComponent<CharacterAttention>();
    }

    public override bool Decide()
    {
        if (_characterAttention != null)
        {
            return _characterAttention.IsAttack; 
        }
        return false;
    }
}
