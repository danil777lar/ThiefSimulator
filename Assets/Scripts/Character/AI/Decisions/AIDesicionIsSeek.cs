using System.Collections;
using System.Collections.Generic;
using Larje.Character.AI;
using UnityEngine;

public class AIDesicionIsSeek : AIDecision
{
    private EnemyAttention _attention;
    
    public override bool Decide()
    {
        return _attention.CurrentState == EnemyAttention.AttentionState.Suspicious;
    }

    protected override void OnInitialized()
    {
        _attention = Brain.Owner.GetComponent<EnemyAttention>();
    }
}
