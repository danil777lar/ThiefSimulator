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

    private ActualSpeedCharacterMovement _movement;
    private MoveBasedCharacterOrientation3D _orientation;
    private Pullable _nearestPullable;
    private Pullable _currentPullable;
    
    protected const string _pullingAnimationParameterName = "Pulling";
    protected int _pullingAnimationParameter;
    
    public PlayerAction[] Actions { get; private set; }

    protected override void Initialization()
    {
        base.Initialization();
        _movement = _character.FindAbility<ActualSpeedCharacterMovement>();
        _orientation = _character.FindAbility<MoveBasedCharacterOrientation3D>();

        BuildActions();
    }

    public override void ProcessAbility()
    {
        base.ProcessAbility();
        if (_currentPullable == null)
        {
            TryFindCargo();
        }
    }

    private void FixedUpdate()
    {
        if (_currentPullable)
        {
            cargoPullPoint.transform.localPosition = Vector3.zero;
            cargoPullPoint.transform.LookAt(_currentPullable.AttachPoint.position);
            cargoPullPoint.transform.position += cargoPullPoint.transform.forward * attachDistance;
            
            _movement.SetLimit(cargoPullPoint.transform.forward, 270f);

            if (Vector3.Distance(_currentPullable.AttachPoint.position, cargoPullPoint.position) > detachDistance)
            {
                ForceDetachCurrentPullable();
            }
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CanAttachPullable())
            {
                TryAttachPullable();   
            }
            else if (CanDetachPullable())
            {
                TryDetachPullable();
            }
        }
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
        actions.Add(new PlayerAction(TryAttachPullable, CanAttachPullable, () => 0.2f, null));
        actions.Add(new PlayerAction(TryDetachPullable, CanDetachPullable, () => 0.2f, null));
        Actions = actions.ToArray();
    }

    private void TryFindCargo()
    {
        List<Pullable> cargos = new List<Pullable>();
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, findDistance,cargosMask);
        foreach (Collider cargoCollider in  colliders)
        {
            if (cargoCollider.attachedRigidbody != null && cargoCollider.attachedRigidbody.TryGetComponent(out Pullable cargo))
            {
                cargos.Add(cargo);
            }
        }

        if (cargos.Count > 0)
        {
            _nearestPullable = cargos.OrderBy(x => 
                Vector3.Distance(transform.position, x.NearestAttachToPoint(transform.position).position))
                .First();
        }
        else
        {
            _nearestPullable = null;
        }
    }

    private bool CanAttachPullable()
    {
        return (_nearestPullable != null && _currentPullable == null);
    }
    
    private bool CanDetachPullable()
    {
        return _currentPullable != null;
    }

    private void TryAttachPullable()
    {
        if (CanAttachPullable())
        {
            _currentPullable = _nearestPullable;

            Transform attachPoint = _currentPullable.NearestAttachToPoint(transform.position);
            _currentPullable.Attach(cargoPullPoint, attachPoint);

            float totalSpeedMultiplier = speedMultiplier;
            totalSpeedMultiplier *= 1f - Mathf.Clamp01(_currentPullable.Rigidbody.mass / maxMass);
            _movement.MovementSpeedMultiplier = totalSpeedMultiplier;
            _orientation.forceTarget = cargoPullPoint.transform;

            _currentPullable.EventForceDetach += ForceDetachCurrentPullable;
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





















