using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Tools.TopDownEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AIActionMoveTowardsTargetNavMesh : AIAction
{
	[SerializeField] private float pathShiftDistance = 0f;
	[Space] 
	[SerializeField] private float maximumWalkAngle = 90f; 
	[SerializeField] private float minimumDistance = 1f;
	[SerializeField] private float minimumDistanceToCorner = 1f;
	
	private Transform _orientationTarget;
	private NavMeshPath _path;
	private CharacterMovement _characterMovement;
	private CoreCharacterOrientation3D _characterOrientation;
	
	public override void Initialization()
	{
		if (!ShouldInitialize)
		{
			return;
		}
		
		base.Initialization();

		_path = new NavMeshPath();
		_characterMovement = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterMovement>();
		_characterOrientation = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CoreCharacterOrientation3D>();

		if (_orientationTarget == null)
		{
			_orientationTarget = new GameObject().transform;
			_orientationTarget.gameObject.name = "AIActionMoveTowardsTargetNavMesh: Orientation Target";
			_orientationTarget.SetParent(transform);
		}
	}

	public override void OnEnterState()
	{
		base.OnEnterState();
		
		_characterOrientation.forceTarget = _orientationTarget;
	}
	
	public override void OnExitState()
	{
		base.OnExitState();

		_characterMovement?.SetHorizontalMovement(0f);
		_characterMovement?.SetVerticalMovement(0f);
		
		_characterOrientation.forceTarget = null;
	}

	public override void PerformAction()
	{
		SetOrientationTargetPosition();
		Move();
		
		CheckDistanceLimits();
		CheckAngleLimits();
	}

	protected virtual void SetOrientationTargetPosition()
	{
		if (_brain.Target == null)
		{
			return;
		}
		
		Vector3 sourcePosition = _brain.Owner.transform.position; 
		Vector3 targetPosition = _brain.Target.position;
		
		if (NavMesh.CalculatePath(sourcePosition, targetPosition, NavMesh.AllAreas, _path))
		{
			foreach (Vector3 corner in _path.GetShiftedCorners(pathShiftDistance))
			{
				targetPosition = corner;
				if (Vector3.Distance(targetPosition, _brain.Owner.transform.position) >= minimumDistanceToCorner)
				{
					break;
				}
			}
		}
		
		_orientationTarget.position = targetPosition;
	}
	
	protected virtual void Move()
	{
		Vector2 movementVector = new Vector2();
		movementVector.x = _characterOrientation.CurrentDirection.x;
		movementVector.y = _characterOrientation.CurrentDirection.z;

		_characterMovement.SetMovement(movementVector);
	}

	protected virtual void CheckDistanceLimits()
	{
		if (Vector3.Distance(transform.position, _orientationTarget.position) < minimumDistance)
		{
			_characterMovement.SetHorizontalMovement(0f);
			_characterMovement.SetVerticalMovement(0f);
		}
	}
	
	protected virtual void CheckAngleLimits()
	{
		Vector3 targetDirection = _orientationTarget.position - transform.position; 
		float angle = Vector3.Angle(targetDirection, _characterOrientation.CurrentDirection);
		if (angle >= maximumWalkAngle)
		{
			_characterMovement.SetHorizontalMovement(0f);
			_characterMovement.SetVerticalMovement(0f);
		}
	}

	private void OnDrawGizmos()
	{
		if (_path != null)
		{
			DrawPathGizmo(_path.corners.ToList(), Color.red);
			DrawPathGizmo(_path.GetShiftedCorners(pathShiftDistance), Color.blue);
		}
	}

	private void DrawPathGizmo(List<Vector3> points, Color color)
	{
		if (points.Count > 0)
		{
			Gizmos.color = color;
			Gizmos.DrawLine(transform.position, points[0]);
			for (int i = 0; i < points.Count - 1; i++)
			{
				Gizmos.DrawSphere(points[i], 0.25f);
				Gizmos.DrawLine(points[i], points[i + 1]);
			}
			Gizmos.DrawSphere(points[^1], 0.25f);
		}
	}
}
