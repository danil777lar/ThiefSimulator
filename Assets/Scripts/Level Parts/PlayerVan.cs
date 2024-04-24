using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerVan : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotateSpeed = 45f;
    [SerializeField] private float acceleration = 2f;
    [Space]
    [SerializeField] private float dragMove = 2f;
    [SerializeField] private float dragStop = 5f;

    [Header("Path")] 
    [SerializeField] private float minDistance = 10f;

    private bool _move;
    private Vector3 _currentVelocity;
    private Vector3 _currentDirection;
    private Vector3 _targetPosition;
    
    private Character _player; 
    private SplineComputer _splineComputer;
    
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

    private void FindTargetPosition()
    {
        _targetPosition = _splineComputer.Project(_player.transform.position).position;
        _move = Vector3.Distance(transform.position, _targetPosition) > minDistance;
    }

    private void Rotate()
    {
        float currentRotationSpeed = Mathf.Lerp(0f, rotateSpeed, _currentVelocity.magnitude / speed);
        Vector3 direction = _targetPosition - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 
            currentRotationSpeed * Time.fixedDeltaTime);

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
