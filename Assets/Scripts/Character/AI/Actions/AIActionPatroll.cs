using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines;
using Larje.Character;
using Larje.Character.AI;
using UnityEngine;
using UnityEngine.AI;

public class AIActionPatroll : AIAction
{
    private Transform _targetPosition;
    private Transform _targetOrientation;

    private CharacterOrientation _orientation;
    private IEnemyPatrolProcessor _patrolProcessor;

    public override void PerformAction()
    {
        TryPatrol();
    }

    protected override void OnInitialized()
    {
        base.Initialization();

        _orientation = Brain.Owner.GetComponent<CharacterOrientation>();
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

    protected override void OnEnterState()
    {
        base.OnEnterState();
        Brain.Target = _targetPosition;
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

        // _patrolProcessor.TryGetLookPosition(out Vector3 lookPosition) ? _targetOrientation : null;
        // _orientation.LookTarget.InputDirection.AddValue();
        
        // _targetOrientation.position = lookPosition;
    }
}
