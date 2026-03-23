using System.Collections;
using System.Collections.Generic;
using Larje.Character.AI;
using UnityEngine;

public class AIDecisionIsInSeekArea : AIDecision
{
    [SerializeField] private float distance = 2f;
    
    private EnemyAttention _attention;
    
    public override bool Decide()
    {
        return Vector3.Distance(Brain.Owner.transform.position, _attention.LastAttentionPoint) <= distance;
    }

    protected override void OnInitialized()
    {
        _attention = Brain.Owner.GetComponent<EnemyAttention>();
    }
}
