using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Tools.TopDownEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterPull3D : CharacterAbility
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
    private Pullable _nearestCargo;
    private Pullable _cargo;
    
    protected const string _pullingAnimationParameterName = "Pulling";
    protected int _pullingAnimationParameter;

    protected override void Initialization()
    {
        base.Initialization();
        _movement = _character.FindAbility<ActualSpeedCharacterMovement>();
        _orientation = _character.FindAbility<MoveBasedCharacterOrientation3D>();
    }

    public override void ProcessAbility()
    {
        base.ProcessAbility();
        if (_cargo == null)
        {
            TryFindCargo();
        }
    }

    private void FixedUpdate()
    {
        if (_cargo)
        {
            cargoPullPoint.transform.localPosition = Vector3.zero;
            cargoPullPoint.transform.LookAt(_cargo.AttachPoint.position);
            cargoPullPoint.transform.position += cargoPullPoint.transform.forward * attachDistance;
            
            _movement.SetLimit(cargoPullPoint.transform.forward, 270f);

            if (Vector3.Distance(_cargo.AttachPoint.position, cargoPullPoint.position) > detachDistance)
            {
                ForceDetachCargo();
            }
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_nearestCargo != null && _cargo == null)
            {
                AttachCargo(_nearestCargo);   
            }
            else if (_cargo != null)
            {
                DetachCargo();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = gizmosColor;
            Gizmos.DrawWireSphere(transform.position, findDistance);

            if (_nearestCargo != null)
            {
                Gizmos.DrawSphere(_nearestCargo.NearestAttachToPoint(transform.position).position, 0.2f);
            }
        }   
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
            _nearestCargo = cargos.OrderBy(x => 
                Vector3.Distance(transform.position, x.NearestAttachToPoint(transform.position).position))
                .First();
        }
        else
        {
            _nearestCargo = null;
        }
    }

    private void AttachCargo(Pullable cargo)
    {
        _cargo = cargo;
        
        Transform attachPoint = _cargo.NearestAttachToPoint(transform.position);
        _cargo.Attach(cargoPullPoint, attachPoint);

        float totalSpeedMultiplier = speedMultiplier;
        totalSpeedMultiplier *= 1f - Mathf.Clamp01(_cargo.Rigidbody.mass / maxMass);
        _movement.MovementSpeedMultiplier = totalSpeedMultiplier;
        _orientation.forceTarget = cargoPullPoint.transform;

        _cargo.EventForceDetach += ForceDetachCargo;
    }
    
    private void ForceDetachCargo()
    {
        DetachCargo();
    }
    
    private void DetachCargo()
    {
        _cargo.EventForceDetach -= ForceDetachCargo;
        
        _cargo.Detach();
        _cargo = null;

        if (_orientation.forceTarget == cargoPullPoint.transform)
        {
            _orientation.forceTarget = null;   
        }

        _movement.MovementSpeedMultiplier = 1f;
        _movement.RemoveLimit();
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
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _pullingAnimationParameter, _cargo != null,
            _character._animatorParameters, _character.RunAnimatorSanityChecks);
    }
}





















