using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.AI;

public class AIActionAimToNearestSeekPoint : AIAction
{
    private Transform _target;
    private EnemySeek _seek;
    
    public override void Initialization()
    {
        base.Initialization();

        _seek = _brain.Owner.GetComponent<EnemySeek>();
        
        _target = new GameObject().transform;
        _target.gameObject.name = "Run To Nearest Seek Point Target";
        _target.SetParent(transform);
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        _brain.Target = _target;
    }

    public override void PerformAction()
    {
        if (_seek.TryFindBestPoint(out Vector3 point))
        {
            _target.position = point;
        }
    }
}
