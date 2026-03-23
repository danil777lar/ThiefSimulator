using System.Collections;
using System.Collections.Generic;
using Larje.Character.AI;
using UnityEngine;

public class AIDecisionPlayerInVision : AIDecision
{
    private EnemyAttention _attention;
    
    public override bool Decide()
    {
        return true;
    }

    protected override void OnInitialized()
    {
        _attention = Brain.Owner.GetComponent<EnemyAttention>();
    }
}
