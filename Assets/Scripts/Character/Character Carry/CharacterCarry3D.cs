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
    [Space(50f)] 
    [SerializeField] private float weightCapacity;
    [SerializeField] private float maxWeightSpeedMultiplier;
    
    [Header("Find")]
    [SerializeField] private float findDistance;
    [SerializeField] private LayerMask carryableMask;
    
    [Header("Carry")]
    [SerializeField] private Transform carryableAttachPoint;
    
    [Header("Gizmos")]
    [SerializeField] private bool drawGizmos;
    [SerializeField] private Color gizmosColor;
    
    [Header("Action Icons")] 
    [SerializeField] private Sprite takeIcon;
    [SerializeField] private Sprite dropIcon;

    private Carryable _nearestCarryable;
    private CoreCharacterMovement _movement;
    private List<Carryable> _currentCarryables;

    private List<Func<float>> _weightCapacityMultipliers = new List<Func<float>>();

    protected int _carryAnimationParameter;
    protected const string _carryAnimationParameterName = "Carry";
    
    public bool HasCarryable => _currentCarryables.Count > 0;
    public float WeightCapacity => weightCapacity * GetWeightCapacityMultiplier();
    public float CurrentWeight => _currentCarryables.Sum(x => x.Weight);
    public float WeightPercent => Mathf.Clamp01(CurrentWeight / WeightCapacity);
    public PlayerAction[] Actions { get; private set; }

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

    public override void UpdateAnimator()
    {
        base.UpdateAnimator();
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _carryAnimationParameter, 
            _currentCarryables is { Count: > 0 },
            _character._animatorParameters, _character.RunAnimatorSanityChecks);
    }
    
    public void TryAddWeightCapacityMultiplier(Func<float> multiplier)
    {
        if (!_weightCapacityMultipliers.Contains(multiplier))
        {
            _weightCapacityMultipliers.Add(multiplier);
        }
    }
    
    public void TryRemoveWeightCapacityMultiplier(Func<float> multiplier)
    {
        if (_weightCapacityMultipliers.Contains(multiplier))
        {
            _weightCapacityMultipliers.Remove(multiplier);
        }
    }

    protected override void Initialization()
    {
        base.Initialization();
        _movement = _character.FindAbility<CoreCharacterMovement>();
        _movement.TryAddSpeedMultiplier(GetSpeedMultiplier);
        _currentCarryables = new List<Carryable>();
        
        BuildActions();
    }

    protected override void InitializeAnimatorParameters()
    {
        base.InitializeAnimatorParameters();
        RegisterAnimatorParameter(_carryAnimationParameterName, AnimatorControllerParameterType.Bool,
            out _carryAnimationParameter);
    }

    private void FixedUpdate()
    {
        _currentCarryables?.ForEach(x => x.UpdatePosition(Time.fixedDeltaTime, _movement.ActualSpeedPercent));
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
        return _nearestCarryable != null && AbilityAuthorized && CurrentWeight < WeightCapacity;
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
            _nearestCarryable.Take(attachPoint);
            _currentCarryables.Add(_nearestCarryable);
        }
    }

    private float GetSpeedMultiplier()
    {
        return Mathf.Lerp(1f, maxWeightSpeedMultiplier, WeightPercent);
    }

    private float GetWeightCapacityMultiplier()
    {
        float result = 1f;
        _weightCapacityMultipliers.ForEach(x => result *= x());
        return result;
    }
}
