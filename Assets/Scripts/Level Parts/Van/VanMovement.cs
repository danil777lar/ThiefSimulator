using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class VanMovement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float dragMove = 2f;
    [SerializeField] private float dragStop = 5f;
    [SerializeField] private float acceleration = 2f;
    [Space]
    [SerializeField] private float angularSpeed = 45f;
    [SerializeField] private float angularDragMove = 1f;
    [SerializeField] private float angularDragStop = 1f;

    [Header("Path")] 
    [SerializeField] private float startMoveDistance = 15f;
    [SerializeField] private float stopMoveDistance = 2f;

    private bool _move;
    private float _currentRotationSpeed;
    private Vector3 _currentVelocity;
    private Vector3 _currentDirection;
    private Vector3 _targetPosition;
    
    private Character _player; 
    private SplineComputer _splineComputer;

    private Vector3 TrajectoryCenter => Vector3.zero;
    private Vector3 PlayerPoint => _splineComputer.Project(_player.transform.position).position;
    
    private void Start()
    {
        _splineComputer = GetComponentInParent<SplineComputer>();
        _player = FindObjectsOfType<Character>().ToList().Find(x => x.CharacterType == Character.CharacterTypes.Player);
    }
    
    private void FixedUpdate()
    {
        FindTargetPosition();
        Rotate();
        Move();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_targetPosition, 0.25f);
    }

    private void FindTargetPosition()
    {
        Vector3 playerDirection = PlayerPoint - TrajectoryCenter;
        Vector3 selfDirection = _splineComputer.Project(transform.position).position - TrajectoryCenter;
        float angle = Vector3.SignedAngle(playerDirection, selfDirection, Vector3.up);
        float angleDelta = 20f * (angle > 0f ? -1f : 1f);
        
        selfDirection = Quaternion.Euler(Vector3.up * angleDelta) * selfDirection;
        _targetPosition = _splineComputer.Project(TrajectoryCenter + (selfDirection.normalized * 100f)).position;

        float distance = Vector3.Distance(transform.position, PlayerPoint); 
        if (_move && distance <= stopMoveDistance)
        {
            _move = false;
        }

        if (!_move && distance >= startMoveDistance)
        {
            _move = true;
        }
    }

    private void Rotate()
    {
        float angularSpeedDelta = Mathf.Lerp(0f, angularSpeed, _currentVelocity.magnitude / speed);
        Vector3 direction = _targetPosition - transform.position;
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        angularSpeedDelta *= (angle < 0f ? -1f : 1f);
        angularSpeedDelta *= 1f - Mathf.Clamp01((Mathf.Abs(angle) - 180f) / 180f);
        
        _currentRotationSpeed += angularSpeedDelta * Time.fixedDeltaTime;
        _currentRotationSpeed *= 1f - Time.fixedDeltaTime * (_move ? angularDragMove : angularDragStop);
        
        transform.rotation *= Quaternion.Euler(Vector3.up * (_currentRotationSpeed * Time.fixedDeltaTime));

        if (_move)
        {
            _currentDirection = transform.forward;
        } 
    }
    
    private void Move()
    {
        float speedDelta = _move ? acceleration : 0f;
        
        _currentVelocity += _currentDirection * (speedDelta * Time.fixedDeltaTime);
        _currentVelocity = _currentVelocity.normalized * Mathf.Clamp(_currentVelocity.magnitude, 0f, speed);

        float drag = dragStop;
        if (_move)
        {
            drag = (1f - (Mathf.Abs(90f - Vector3.Angle(_currentVelocity, transform.forward)) / 90f)) * dragMove;
        }
        _currentVelocity *= 1f - Time.fixedDeltaTime * drag;
        transform.position += _currentVelocity * Time.deltaTime;
    }
}
