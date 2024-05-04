using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.AI;

public class AIActionMoveTowardsTargetNavMesh : AIAction
{
	[SerializeField] private float minimumDistance = 1f;
	[SerializeField] private float minimumDistanceToCorner = 1f;
	[Header("Smooth Rotation")]
	[SerializeField] private bool useSmoothRotation;
	[SerializeField] private float rotationSpeed = 180f;

	private float _lastPerformTime;
	private Vector3 _currentDirection;
	private Vector3 _directionToTarget;
	private NavMeshPath _path;
	private CharacterMovement _characterMovement;
	private Vector2 _movementVector;
	
	public override void Initialization()
	{
		if (!ShouldInitialize)
		{
			return;
		}
		
		base.Initialization();

		_path = new NavMeshPath();
		_characterMovement = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterMovement>();
	}

	public override void OnEnterState()
	{
		base.OnEnterState();
		_lastPerformTime = Time.time;
		_currentDirection = Vector3.forward;
	}

	public override void PerformAction()
	{
		Move();
	}
	
	protected virtual void Move()
	{
		float deltaTime = Time.time - _lastPerformTime;
		_lastPerformTime = Time.time;
		
		if (_brain.Target == null)
		{
			return;
		}
		
		Vector3 sourcePosition = _brain.Owner.transform.position; 
		Vector3 targetPosition = _brain.Target.position;
		
		if (NavMesh.CalculatePath(sourcePosition, targetPosition, NavMesh.AllAreas, _path))
		{
			foreach (Vector3 corner in _path.corners)
			{
				targetPosition = corner;
				if (Vector3.Distance(targetPosition, _brain.Owner.transform.position) >= minimumDistanceToCorner)
				{
					break;
				}
			}
		}
		
		Vector3 direction = targetPosition - sourcePosition;
		if (useSmoothRotation)
		{
			_currentDirection = Vector3.RotateTowards(_currentDirection, direction,
				Mathf.Deg2Rad * rotationSpeed * deltaTime, 0f).normalized;
		}
		else
		{
			_currentDirection = direction;
		}
		targetPosition = sourcePosition + (_currentDirection * direction.magnitude);

		_directionToTarget = targetPosition - sourcePosition;
		_movementVector.x = _directionToTarget.x;
		_movementVector.y = _directionToTarget.z;
		_characterMovement.SetMovement(_movementVector);

		if (Mathf.Abs(this.transform.position.x - targetPosition.x) < minimumDistance)
		{
			_characterMovement.SetHorizontalMovement(0f);
		}

		if (Mathf.Abs(this.transform.position.z - targetPosition.z) < minimumDistance)
		{
			_characterMovement.SetVerticalMovement(0f);
		}
	}

	public override void OnExitState()
	{
		base.OnExitState();

		_characterMovement?.SetHorizontalMovement(0f);
		_characterMovement?.SetVerticalMovement(0f);
	}

	private void OnDrawGizmos()
	{
		if (_path != null && _path.corners.Length > 0)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, _path.corners[0]);
			for (int i = 0; i < _path.corners.Length - 1; i++)
			{
				Gizmos.DrawSphere(_path.corners[i], 0.25f);
				Gizmos.DrawLine(_path.corners[i], _path.corners[i + 1]);
			}
			Gizmos.DrawSphere(_path.corners[^1], 0.25f);
		}
	}
}
