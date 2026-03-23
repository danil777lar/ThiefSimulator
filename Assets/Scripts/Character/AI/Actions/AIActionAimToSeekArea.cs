using System.Collections;
using System.Collections.Generic;
using Larje.Character.AI;
using UnityEngine;

public class AIActionAimToSeekArea : AIAction
{
    private Transform _target;
    private EnemyAttention _attention;
    
    public override void PerformAction()
    {
        _target.position = _attention.LastAttentionPoint;
    }

    protected override void OnInitialized()
    {
        base.Initialization();
        _attention = Brain.Owner.GetComponentInChildren<EnemyAttention>();

        _target = new GameObject().transform;
        _target.gameObject.name = "Run To Seek Area Target";
        _target.SetParent(transform);
    }

    protected override void OnEnterState()
    {
        base.OnEnterState();
        Brain.Target = _target;
    }

}
