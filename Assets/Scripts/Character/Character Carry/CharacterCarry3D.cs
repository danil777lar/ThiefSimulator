using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Larje.Character;
using Larje.Character.Abilities;
using UnityEditor;
using UnityEngine;

public class CharacterCarry3D : CharacterAbility, IPlayerActionSource
{
    [Space(50f)] 
    [SerializeField] private bool autoGrab;
    [SerializeField] private float autoGrabDelay = 0.5f;
    [SerializeField] private float weightCapacity;
    [SerializeField] private float maxWeightSpeedMultiplier;
    
    [Header("Find")]
    [SerializeField] private float findDistance;
    [SerializeField] private LayerMask carryableMask;
    [SerializeField] private LayerMask obstacleMask;
    
    [Header("Carry")]
    [SerializeField] private Transform carryableAttachPoint;

    [Header("Sounds")]
    [SerializeField] private SoundSettings takeSound;
    [SerializeField] private SoundSettings dropSound;
    
    [Header("Gizmos")]
    [SerializeField] private bool drawGizmos;
    [SerializeField] private Color gizmosColor;
    
    [Header("Action Icons")] 
    [SerializeField] private Sprite takeIcon;
    [SerializeField] private Sprite dropIcon;

    private float _autoGrabTimer;
    private Carryable _nearestCarryable;
    private CharacterWalk _movement;
    private List<Carryable> _currentCarryables;

    private List<Func<float>> _weightCapacityMultipliers = new List<Func<float>>();

    protected int _carryAnimationParameter;
    protected const string _carryAnimationParameterName = "Carry";
    
    public bool HasCarryable => _currentCarryables.Count > 0;
    public float WeightCapacity => weightCapacity * GetWeightCapacityMultiplier();
    public float CurrentWeight => _currentCarryables.Sum(x => x.Weight);
    public float WeightPercent => Mathf.Clamp01(CurrentWeight / WeightCapacity);

    public IReadOnlyCollection<Carryable> CurrentCarryables => _currentCarryables.AsReadOnly();

    public PlayerAction[] Actions { get; private set; }

    public Carryable TryDrop(bool blockTaking = false)
    {
        if (CanDrop())
        {
            Carryable carryToDrop = _currentCarryables.Last();
            carryToDrop.Drop(blockTaking);
            _currentCarryables.Remove(carryToDrop);

            DOVirtual.DelayedCall(UnityEngine.Random.Range(0f, 0.25f), () => dropSound.Play(), ignoreTimeScale: true);

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
        _currentCarryables.Clear();
        return carryables;
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

    protected override void OnInitialized()
    {
        _movement = character.FindAbility<CharacterWalk>();
        _movement.WalkMultiplier.AddValue(GetSpeedMultiplier);
        _currentCarryables = new List<Carryable>();
        
        BuildActions();
    }

    private void Update()
    {
        if (!Permitted)
        {
            DropAll();
            return;
        }
        
        TryFindCarryable();
        TryAutoGrab();
    }

    private void FixedUpdate()
    {
        if (_autoGrabTimer > 0f)
        {
            _autoGrabTimer -= Time.fixedDeltaTime;
        }

        _currentCarryables?.ForEach(x => x.UpdatePosition(Time.fixedDeltaTime, _movement.SpeedPercent));
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

    private void TryAutoGrab()
    {
        if (autoGrab && CanTake())
        {
            TryTake();
            _autoGrabTimer = autoGrabDelay;
        }
    }

    private void BuildActions()
    {
        List<PlayerAction> actions = new List<PlayerAction>();
        if (!autoGrab)
        {
            actions.Add(new PlayerAction(TryTake, CanTake, () => 0.2f, takeIcon));
            actions.Add(new PlayerAction(() => TryDrop(), CanDrop, () => 0.2f, dropIcon));
        }
        Actions = actions.ToArray();
    }

    private void TryFindCarryable()
    {
        _nearestCarryable = null;

        Vector3 origin = character.transform.position;
        List<Carryable> carryables = PhysicsUtility.FindObjectsInRange<Carryable>(origin, findDistance, carryableMask, obstacleMask)
            .Keys.ToList().FindAll(x => x.CanBeTaken);

        if (carryables.Count > 0)
        {
            _nearestCarryable = carryables.OrderBy(x => Vector3.Distance(character.transform.position, x.transform.position)).First();
        }
    }

    private bool CanTake()
    {
        bool result = _nearestCarryable != null && Permitted && CurrentWeight < WeightCapacity;
        if (autoGrab)
        {
            result &= _autoGrabTimer <= 0f;
        }
        return result;
    }

    private bool CanDrop()
    {
        return _currentCarryables.Count > 0;
    }

    private void TryTake()
    {
        if (CanTake())
        {
            Transform attachPoint = _currentCarryables.Count > 0 ? _currentCarryables.Last().TopPoint : carryableAttachPoint;
            _nearestCarryable.Take(attachPoint);
            _currentCarryables.Add(_nearestCarryable);

            takeSound.Play();
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
