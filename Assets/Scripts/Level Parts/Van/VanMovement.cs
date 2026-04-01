using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Dreamteck.Splines;
using Larje.Character;
using Larje.Core;
using Larje.Core.Services;
using ProjectConstants;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class VanMovement : MonoBehaviour, ILevelStartHandler, ILevelEndHandler
{
    [Header("Speed")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float dragMove = 2f;
    [SerializeField] private float dragStop = 5f;
    [Space]
    [SerializeField] private float angularSpeed = 45f;
    [SerializeField] private float angularDragMove = 1f;
    [SerializeField] private float angularDragStop = 1f;
    [SerializeField] private AnimationCurve angularSpeedCurve;

    [Header("Path")] 
    [SerializeField] private float startMoveDistance = 15f;
    [SerializeField] private float stopMoveDistance = 2f;
    [SerializeField] private float boxcastDistance = 3f;
    [SerializeField] private LayerMask boxcastMask;

    [Header("Start Animation")] 
    [SerializeField] private float startAnimationDistance = 20f;
    [SerializeField] private float startAnimationDurationModifier = 1f;

    [Header("Sounds")]
    [SerializeField] private SoundSettings soundIdle;
    [SerializeField] private SoundSettings soundMove;

    [InjectService] private GameEventService _gameEventService;
    [InjectService] private IGameStateService _stateService;
    [InjectService] private IPlayerProviderService _playerProviderService;

    private bool _move;
    private bool _startedAnimationComplete;
    private bool _winAnimationPlaying;

    private float _currentRotationSpeed;

    private float _soundIdleVolume;
    private float _soundMoveVolume;

    private Vector3 _startPosition;
    private Vector3 _currentVelocity;
    private Vector3 _currentDirection;
    private Vector3 _targetPosition;

    private Rigidbody _rigidbody;
    private BoxCollider _collider;
    private Character _player; 
    private SplineComputer _splineComputer;

    private Vector3 TrajectoryCenter => Vector3.zero;
    private Vector3 PlayerPoint => _splineComputer.Project(_player.transform.position).position;
    public bool IsMoving => _move;

    public event Action EventInitialized;
    public event Action EventStartMove; 
    public event Action EventStopMove; 
    public event Action EventStartAnimationComplete; 
    
    public void OnLevelStarted(LevelProcessor.StartData data)
    {
        
    }

    public void OnLevelEnded(LevelProcessor.StopData data)
    {
        if (data.StopType == LevelStopType.Win)
        {
            _winAnimationPlaying = true;
        }
    }
    
    private void Start()
    {
        DIContainer.InjectTo(this);

        _gameEventService.Subscribe<LevelEventPreStart>(OnLevelPrestarted);

        _splineComputer = GetComponentInParent<SplineComputer>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<BoxCollider>();
        _playerProviderService.TryGetPlayer(out _player);

        _startPosition = transform.position;
        transform.position -= transform.forward * startAnimationDistance;
        
        soundIdle.Play(s => s.SetTarget(this).SetLoop(true).SetPosition(t => transform.position).SetSpatialBlend(t => 1f).SetVolume(t => _soundIdleVolume));
        soundMove.Play(s => s.SetTarget(this).SetLoop(true).SetPosition(t => transform.position).SetSpatialBlend(t => 1f).SetVolume(t => _soundMoveVolume));

        EventInitialized?.Invoke();
    }

    private void OnDisable()
    {
        this.SoundServiceStop();
        _gameEventService?.Unsubscribe<LevelEventPreStart>(OnLevelPrestarted);
    }
    
    private void FixedUpdate()
    {
        if (_startedAnimationComplete)
        {
            FindTargetPosition();
            Rotate();
            Move();
        }
    }

    private void Update()
    {
        bool useSound = _stateService.CurrentState == GameStates.Playing || _stateService.CurrentState == GameStates.Cutscene;
        _soundIdleVolume = Mathf.Lerp(_soundIdleVolume, !_move && useSound ? 1f : 0f, Time.deltaTime * 5f);
        _soundMoveVolume = Mathf.Lerp(_soundMoveVolume, _move && useSound ? 1f : 0f, Time.deltaTime * 5f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_targetPosition, 0.25f);
    }

    private void OnLevelPrestarted(LevelEventPreStart prestartEvent)
    {
        EventStartMove?.Invoke();
        transform.DOMove(_startPosition, prestartEvent.StartDelay * startAnimationDurationModifier)
            .SetUpdate(UpdateType.Fixed)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                _startedAnimationComplete = true;
                EventStartAnimationComplete?.Invoke();
                EventStopMove?.Invoke();
            });    
    }

    private void FindTargetPosition()
    {
        _targetPosition = _winAnimationPlaying ? GetTargetPositionWin() : GetTargetPositionDefault();
        float distance = Vector3.Distance(transform.position, _winAnimationPlaying ? _targetPosition : PlayerPoint); 
        
        if (_move && distance <= stopMoveDistance)
        {
            _move = false;
            EventStopMove?.Invoke();
        }

        if (!_move && distance >= startMoveDistance)
        {
            _move = true;
            EventStartMove?.Invoke();
        }
    }

    private Vector3 GetTargetPositionDefault()
    {
        Vector3 playerDirection = PlayerPoint - TrajectoryCenter;
        Vector3 selfDirection = _splineComputer.Project(transform.position).position - TrajectoryCenter;
        float angle = Vector3.SignedAngle(playerDirection, selfDirection, Vector3.up);
        float angleDelta = 20f * (angle > 0f ? -1f : 1f);
        
        selfDirection = Quaternion.Euler(Vector3.up * angleDelta) * selfDirection;
        return _splineComputer.Project(TrajectoryCenter + (selfDirection.normalized * 100f)).position;        
    }

    private Vector3 GetTargetPositionWin()
    {
        Vector3 direction = transform.position - TrajectoryCenter;
        return TrajectoryCenter + direction.normalized * 100f;
    }

    private void Rotate()
    {
        Vector3 direction = _move ? 
            _targetPosition - transform.position :
            transform.position - TrajectoryCenter;
        
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        float angularSpeedDelta = angularSpeed;
        angularSpeedDelta *= _currentVelocity.magnitude / speed;
        angularSpeedDelta *= angularSpeedCurve.Evaluate(1f - Mathf.Clamp01((Mathf.Abs(angle) - 180f) / 180f));
        angularSpeedDelta *= (angle < 0f ? -1f : 1f);
        
        _currentRotationSpeed += angularSpeedDelta * Time.fixedDeltaTime;
        _currentRotationSpeed *= 1f - Time.fixedDeltaTime * (_move ? angularDragMove : angularDragStop);
        
        ApplyRotation();

        if (_move)
        {
            _currentDirection = transform.forward;
        } 
    }

    private void ApplyRotation()
    {
        if (_rigidbody)
        {
            _rigidbody.angularVelocity = Vector3.up * (_currentRotationSpeed * Mathf.Deg2Rad);
        }
        else
        {
            transform.rotation *= Quaternion.Euler(Vector3.up * (_currentRotationSpeed * Time.fixedDeltaTime));   
        }
    }
    
    private void Move()
    {
        float speedDelta = _move ? acceleration : 0f;
        speedDelta *= HasForwardObstacle() ? -1f : 1f;
        
        _currentVelocity += _currentDirection * (speedDelta * Time.fixedDeltaTime);
        _currentVelocity = _currentVelocity.normalized * Mathf.Clamp(_currentVelocity.magnitude, 0f, speed);

        float drag = dragStop;
        if (_move)
        {
            drag = (1f - (Mathf.Abs(90f - Vector3.Angle(_currentVelocity, transform.forward)) / 90f)) * dragMove;
        }
        _currentVelocity *= 1f - Time.fixedDeltaTime * drag;
        
        ApplyMovement();
    }
    
    private void ApplyMovement()
    {
        if (_rigidbody)
        {
            _rigidbody.linearVelocity = _currentVelocity;
        }
        
        else
        {
            transform.position += _currentVelocity * Time.fixedDeltaTime;
        }
    }

    private bool HasForwardObstacle()
    {
        Vector3 from = transform.position + _collider.center - transform.forward;
        return Physics.BoxCast(from, _collider.size / 2f, transform.forward, 
                transform.rotation, boxcastDistance, boxcastMask);
    }
}
