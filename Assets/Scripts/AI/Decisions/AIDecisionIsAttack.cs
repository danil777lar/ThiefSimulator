using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class AIDecisionIsAttack : AIDecision
{
    private EnemyAttention _enemyAttention;

    public override void Initialization()
    {
        base.Initialization();
        _enemyAttention = _brain.Owner.GetComponent<EnemyAttention>();
    }

    public override bool Decide()
    {
        if (_enemyAttention != null)
        {
            return _enemyAttention.IsAttack; 
        }
        return false;
    }
}
