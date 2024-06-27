using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines;
using Larje.Core.Tools.TopDownEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.AI;

public class AIActionPatroll : AIAction
{
    private Transform _targetPosition;
    private Transform _targetOrientation;

    private CoreCharacterOrientation3D _orientation;
    private IEnemyPatrolProcessor _patrolProcessor;

    public override void Initialization()
    {
        base.Initialization();

        _orientation = _brain.Owner.GetComponent<CoreCharacterOrientation3D>();
        _patrolProcessor = GetComponentInParent<IEnemyPatrolProcessor>();
        
        if (_targetPosition == null)
        {
            CreateTarget(out _targetPosition, "Patroll Target Position");
        }
        if (_targetOrientation == null)
        {
            CreateTarget(out _targetOrientation, "Patroll Target Orientation");
        }
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        _brain.Target = _targetPosition;
    }

    public override void PerformAction()
    {
        TryPatrol();
    }

    private void CreateTarget(out Transform target, string targetName)
    {
        target = new GameObject().transform;
        target.gameObject.name = targetName;
        target.SetParent(transform);
    }

    private void TryPatrol()
    {
        if (_patrolProcessor.TryGetPosition(out Vector3 position))
        {
            _targetPosition.position = position;
        }

        _orientation.forceLookTarget =
            _patrolProcessor.TryGetLookPosition(out Vector3 lookPosition) 
                ? _targetOrientation : null;
        
        _targetOrientation.position = lookPosition;
    }
}
