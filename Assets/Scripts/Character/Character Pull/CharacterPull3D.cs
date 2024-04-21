using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Tools.TopDownEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterPull3D : CharacterAbility, IPlayerActionSource
{
    [Header("Speed")]
    [SerializeField] private float maxMass;
    [SerializeField] private float speedMultiplier;
    [Space]
    [SerializeField] private float findDistance;
    [SerializeField] private float attachDistance;
    [SerializeField] private float detachDistance;
    [SerializeField] private LayerMask cargosMask;
    [SerializeField] private Transform cargoPullPoint;
    [Header("Gizmos")]
    [SerializeField] private bool drawGizmos;
    [SerializeField] private Color gizmosColor;
    [Header("Action Icons")] 
    [SerializeField] private Sprite attachIcon;
    [SerializeField] private Sprite detachIcon;

    private CoreCharacterMovement _movement;
    private CoreCharacterOrientation3D _orientation;
    private Pullable _nearestPullable;
    private Pullable _currentPullable;
    
    protected const string _pullingAnimationParameterName = "Pulling";
    protected int _pullingAnimationParameter;
    
    public PlayerAction[] Actions { get; private set; }

    protected override void Initialization()
    {
        base.Initialization();
        _movement = _character.FindAbility<CoreCharacterMovement>();
        _orientation = _character.FindAbility<CoreCharacterOrientation3D>();

        BuildActions();
    }

    public override void ProcessAbility()
    {
        base.ProcessAbility();
        
        if (!AbilityAuthorized)
        {
            TryDetachPullable();
            return;
        }

        TryFindPullable();
    }

    private void FixedUpdate()
    {
        if (!AbilityAuthorized)
        {
            return;
        }
        
        TryProcessPullable();
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = gizmosColor;
            Gizmos.DrawWireSphere(transform.position, findDistance);

            if (_nearestPullable != null)
            {
                Gizmos.DrawSphere(_nearestPullable.NearestAttachToPoint(transform.position).position, 0.2f);
            }
        }   
    }

    private void BuildActions()
    {
        List<PlayerAction> actions = new List<PlayerAction>();
        actions.Add(new PlayerAction(TryAttachPullable, CanAttachPullable, () => 0.2f, attachIcon));
        actions.Add(new PlayerAction(TryDetachPullable, CanDetachPullable, () => 0.2f, detachIcon));
        Actions = actions.ToArray();
    }

    private void TryFindPullable()
    {
        if (_currentPullable != null)
        {
            return;
        }

        List<Pullable> pullables = PhysicsUtility.FindObjectsInRange<Pullable>
                (transform.position, findDistance, cargosMask, _controller3D.ObstaclesLayerMask)
            .Keys.ToList();
        
        if (pullables.Count > 0)
        {
            _nearestPullable = pullables.OrderBy(x => 
                Vector3.Distance(transform.position, x.NearestAttachToPoint(transform.position).position))
                .First();
        }
        else
        {
            _nearestPullable = null;
        }
    }
    
    private void TryProcessPullable()
    {
        if (_currentPullable)
        {
            cargoPullPoint.transform.localPosition = Vector3.zero;
            cargoPullPoint.transform.LookAt(_currentPullable.AttachPoint.position);

            Vector3 pullPointPosition = cargoPullPoint.transform.position;
            pullPointPosition += cargoPullPoint.transform.forward * attachDistance;
            pullPointPosition.y = transform.position.y;
            cargoPullPoint.transform.position = pullPointPosition;
            
            _movement.SetLimit(cargoPullPoint.transform.forward, 270f);
            if (Vector3.Distance(_currentPullable.AttachPoint.position, cargoPullPoint.position) > detachDistance)
            {
                ForceDetachCurrentPullable();
            }
            else
            {
                _currentPullable.UpdatePosition();
            }
        }
    }

    private bool CanAttachPullable()
    {
        return (AbilityAuthorized && _nearestPullable != null && _currentPullable == null);
    }
    
    private bool CanDetachPullable()
    {
        return _currentPullable != null;
    }

    private void TryAttachPullable()
    {
        if (CanAttachPullable())
        {
            _character.MovementState.ChangeState(CharacterStates.MovementStates.Pulling);
            
            _currentPullable = _nearestPullable;

            Transform attachPoint = _currentPullable.NearestAttachToPoint(transform.position);
            _currentPullable.Attach(cargoPullPoint, attachPoint);

            float totalSpeedMultiplier = speedMultiplier;
            totalSpeedMultiplier *= 1f - Mathf.Clamp01(_currentPullable.Rigidbody.mass / maxMass);
            _movement.MovementSpeedMultiplier = totalSpeedMultiplier;
            _orientation.forceTarget = cargoPullPoint.transform;
            
            TryProcessPullable();

            if (_currentPullable != null)
            {
                _currentPullable.EventForceDetach += ForceDetachCurrentPullable;
            }
        }
    }
    
    private void ForceDetachCurrentPullable()
    {
        TryDetachPullable();
    }
    
    private void TryDetachPullable()
    {
        if (CanDetachPullable())
        {
            _character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);

            _currentPullable.EventForceDetach -= ForceDetachCurrentPullable;

            _currentPullable.Detach();
            _currentPullable = null;

            if (_orientation.forceTarget == cargoPullPoint.transform)
            {
                _orientation.forceTarget = null;
            }

            _movement.MovementSpeedMultiplier = 1f;
            _movement.RemoveLimit();
        }
    }
    
    protected override void InitializeAnimatorParameters()
    {
        base.InitializeAnimatorParameters();
        RegisterAnimatorParameter(_pullingAnimationParameterName, AnimatorControllerParameterType.Bool,
            out _pullingAnimationParameter);
    }

    public override void UpdateAnimator()
    {
        base.UpdateAnimator();
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _pullingAnimationParameter, _currentPullable != null,
            _character._animatorParameters, _character.RunAnimatorSanityChecks);
    }
}





















