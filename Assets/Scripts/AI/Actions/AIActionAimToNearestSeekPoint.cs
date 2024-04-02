using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.AI;

public class AIActionAimToNearestSeekPoint : AIAction
{
    private Transform _target;
    private CharacterSeek _seek;
    
    public override void Initialization()
    {
        base.Initialization();

        _seek = _brain.Owner.GetComponent<CharacterSeek>();
        
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
        if (_seek.SeekPoints is { Count: > 0 })
        {
            Vector3 nearestPoint = _seek.SeekPoints.OrderBy(x =>
            {
                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(_brain.Owner.transform.position, x, NavMesh.AllAreas, path))
                {
                    return path.GetLength();
                }
                return 1000000f;
            }).First();
            
            _target.position = nearestPoint;
        }
    }
}
