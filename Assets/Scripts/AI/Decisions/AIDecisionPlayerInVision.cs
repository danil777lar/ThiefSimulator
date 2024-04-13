using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class AIDecisionPlayerInVision : AIDecision
{
    private EnemyAttention _attention;
    
    public override void Initialization()
    {
        base.Initialization();
        _attention = _brain.Owner.GetComponent<EnemyAttention>();
    }

    public override bool Decide()
    {
        return true;
    }
}
