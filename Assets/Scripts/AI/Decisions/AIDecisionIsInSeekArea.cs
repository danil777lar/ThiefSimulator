using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class AIDecisionIsInSeekArea : AIDecision
{
    [SerializeField] private float distance = 2f;
    
    private EnemyAttention _attention;
    
    public override void Initialization()
    {
        base.Initialization();
        _attention = _brain.Owner.GetComponent<EnemyAttention>();
    }

    public override bool Decide()
    {
        return Vector3.Distance(_brain.Owner.transform.position, _attention.LastAttentionPoint) <= distance;
    }
}
