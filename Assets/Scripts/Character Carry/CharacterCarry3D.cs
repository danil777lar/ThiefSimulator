using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Tools.TopDownEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEditor;
using UnityEngine;

public class CharacterCarry3D : CharacterAbility, IPlayerActionSource
{
    [SerializeField] private float findDistance;
    [SerializeField] private float anchoringValuePerObject;
    [SerializeField] private LayerMask carryableMask;
    [SerializeField] private Transform carryableAttachPoint;
    [Header("Gizmos")]
    [SerializeField] private bool drawGizmos;
    [SerializeField] private Color gizmosColor;
    [Header("Action Icons")] 
    [SerializeField] private Sprite takeIcon;
    [SerializeField] private Sprite dropIcon;

    private ActualSpeedCharacterMovement _movement;
    private Carryable _nearestCarryable;
    private List<Carryable> _currentCarryables;

    protected const string _carryAnimationParameterName = "Carry";
    protected int _carryAnimationParameter;

    public PlayerAction[] Actions { get; private set; }

    protected override void Initialization()
    {
        base.Initialization();
        _movement = _character.FindAbility<ActualSpeedCharacterMovement>();
        _currentCarryables = new List<Carryable>();
        
        BuildActions();
    }

    public override void ProcessAbility()
    {
        base.ProcessAbility();

        if (!AbilityAuthorized)
        {
            DropAll();
            return;
        }
        
        TryFindCarryable();
    }

    public Carryable TryDrop(bool blockTaking = false)
    {
        if (CanDrop())
        {
            Carryable carryToDrop = _currentCarryables.Last();
            carryToDrop.Drop(blockTaking);
            _currentCarryables.Remove(carryToDrop);

            if (_currentCarryables.Count <= 0)
            {
                _character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);
            }

            return carryToDrop;
        }

        return null;
    }
    
    public List<Carryable> DropAll()
    {
        List<Carryable> carryables = new List<Carryable>();
        while (_currentCarryables.Count > 0)
        {
            carryables.Add(TryDrop());
        }
        return carryables;
    }

    private void FixedUpdate()
    {
        _currentCarryables?.ForEach(x => x.UpdatePosition(_movement.ActualSpeedPercent));
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = gizmosColor;
            Gizmos.DrawWireSphere(transform.position, findDistance);

            if (_nearestCarryable != null)
            {
                Gizmos.DrawSphere(_nearestCarryable.transform.position, 0.2f);
            }
        }   
    }

    private void BuildActions()
    {
        List<PlayerAction> actions = new List<PlayerAction>();
        actions.Add(new PlayerAction(TryTake, CanTake, () => 0.2f, takeIcon));
        actions.Add(new PlayerAction(() => TryDrop(), CanDrop, () => 0.2f, dropIcon));
        Actions = actions.ToArray();
    }

    private void TryFindCarryable()
    {
        _nearestCarryable = null;
        List<Carryable> carryables = PhysicsUtility.FindObjectsInRange<Carryable>
            (transform.position, findDistance, carryableMask, _controller3D.ObstaclesLayerMask)
            .Keys.ToList()
            .FindAll(x => x.CanBeTaken);

        if (carryables.Count > 0)
        {
            _nearestCarryable = carryables.OrderBy(x => 
                Vector3.Distance(transform.position, x.transform.position)).First();
        }
    }

    private bool CanTake()
    {
        return _nearestCarryable != null && AbilityAuthorized;
    }
    
    private bool CanDrop()
    {
        return _currentCarryables.Count > 0;
    }

    private void TryTake()
    {
        if (CanTake())
        {
            _character.MovementState.ChangeState(CharacterStates.MovementStates.Carry);
            
            Transform attachPoint = _currentCarryables.Count > 0 ? 
                _currentCarryables.Last().TopPoint : carryableAttachPoint;
            _nearestCarryable.Take(attachPoint, Mathf.Clamp01(_currentCarryables.Count * anchoringValuePerObject));
            _currentCarryables.Add(_nearestCarryable);
        }
    }

    protected override void InitializeAnimatorParameters()
    {
        base.InitializeAnimatorParameters();
        RegisterAnimatorParameter(_carryAnimationParameterName, AnimatorControllerParameterType.Bool,
            out _carryAnimationParameter);
    }

    public override void UpdateAnimator()
    {
        base.UpdateAnimator();
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _carryAnimationParameter, 
            _currentCarryables is { Count: > 0 },
            _character._animatorParameters, _character.RunAnimatorSanityChecks);
    }
}
