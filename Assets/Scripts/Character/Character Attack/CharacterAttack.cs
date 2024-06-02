using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using DG.Tweening;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CharacterAttack : CharacterAbility
{
    [SerializeField] private CharacterAttackConfig config;
    [SerializeField] private LayerMask targetMask;

    [Header("Attack Marker")] 
    [SerializeField] private bool useMarker;
    [SerializeField] private float markerSpawnDistance;
    [SerializeField] private AttackMarker markerPrefab;

    private bool _grabbed;
    private float _attackDelay;
    private CharacterAttack _target;
    private RuntimeAnimatorController _defaultAnimatorController;

    private Dictionary<Collider, CharacterAttack> _targetsDatabase = new Dictionary<Collider, CharacterAttack>();
    private Dictionary<CharacterAttack, AttackMarker> _markers = new Dictionary<CharacterAttack, AttackMarker>();

    public bool IsAttacking { get; private set; }
    public float AttackProgress => 1f - (_attackDelay / config.AttackDelay);
    public Transform CharacterModel => _character.CharacterModel.transform;
    public Health CharacterHealth => _character.CharacterHealth;
    public CharacterAttack Target => _target;

    public override void ProcessAbility()
    {
        base.ProcessAbility();

        if (!AbilityAuthorized || !AbilityPermitted)
        {
            return;
        }

        UpdateMarkers();
        UpdateTarget();
        DecreaseDelay();
        TryStartAttack();
    }

    public void Froze()
    {
        _grabbed = true;
        _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Frozen);
    }

    public void ApplyVictimAnimation(AnimatorOverrideController animations)
    {
        ApplyAnimation(animations, "Victim");
    } 

    protected override void Initialization()
    {
        base.Initialization();

        _defaultAnimatorController = _character.CharacterAnimator.runtimeAnimatorController;

        AnimatorEventReceiver receiver = _character.CharacterAnimator.GetComponent<AnimatorEventReceiver>();
        receiver.EventSendDamage += SendDamage;
        receiver.EventAttackComplete += CompleteAttack;
    }
    
    private void UpdateTarget()
    {
        if (!IsAttacking)
        {
            List<CharacterAttack> targets = FindTargetsInDistance(config.AttackDistance);
            targets = targets.FindAll(CheckLimits);
            targets = targets.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).ToList();
            
            CharacterAttack target = targets.Count > 0 ? targets[0] : null;
            if (_target != target || target == null)
            {
                ResetDelay();
            }
            _target = target;
        }
    }

    private void DecreaseDelay()
    {
        if (_target != null && _attackDelay > 0 && !_grabbed && !IsAttacking)
        {
            _attackDelay -= Time.deltaTime;
        }
    }

    private void ResetDelay()
    {
        _attackDelay = config.AttackDelay;
    }

    private void TryStartAttack()
    {
        if (_attackDelay <= 0 && _target != null && !_grabbed && !IsAttacking && !_target.IsAttacking)
        {
            _target.Froze();
            IsAttacking = true;
            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Frozen);

            TryStartTransition(() =>
            {
                _target.ApplyVictimAnimation(config.Animations);
                if (config.Animations != null)
                {
                    ApplyAnimation(config.Animations, "Killer");
                }
                else
                {
                    SendDamage();
                    CompleteAttack();
                }
            });
        }
    }

    private void TryStartTransition(Action onComplete)
    {
        if (config.UseFixedPosition)
        {
            Vector3 targetPosition = _target.CharacterModel.transform.TransformPoint(config.FixedPosition);
            
            _character.transform.DOMove(targetPosition, config.TransitionToFixedPositionDuration)
                .OnUpdate(() =>
                {
                    _character.CharacterModel.transform.LookAt(_target.CharacterModel.transform.position);
                })
                .SetUpdate(UpdateType.Fixed)
                .OnComplete(() => onComplete?.Invoke());
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    private void UpdateMarkers()
    {
        if (!useMarker)
        {
            return;
        }
        
        List<CharacterAttack> targets = FindTargetsInDistance(markerSpawnDistance);
        List<CharacterAttack> targetsToRemove = new List<CharacterAttack>();
        foreach (CharacterAttack target in _markers.Keys)
        {
            if (!targets.Contains(target))
            {
                _markers[target].Remove();
                targetsToRemove.Add(target);
            }
        }
        
        foreach (CharacterAttack target in targetsToRemove)
        {
            _markers.Remove(target);
        }
        
        foreach (CharacterAttack target in targets)
        {
            if (!_markers.ContainsKey(target))
            {
                AttackMarker marker = Instantiate(markerPrefab, target.CharacterModel);
                float distance = config.AttackDistance;
                float angle = config.VictimDirectionLimit.LimitAngle;
                Vector3 direction = config.VictimDirectionLimit.LimitDirection;
                marker.Init(distance, angle, direction, this, target);
                _markers.Add(target, marker);
            }
        }
    }

    private List<CharacterAttack> FindTargetsInDistance(float distance)
    {
        List<CharacterAttack> result = new List<CharacterAttack>();
        Physics.OverlapSphere(transform.position, distance, targetMask)
            .ToList().ForEach(x =>
            {
                if (TryGetTarget(x, out CharacterAttack target))
                {
                    result.Add(target);
                } 
            });

        return result;
    }
    
    private bool TryGetTarget(Collider collider, out CharacterAttack target)
    {
        if (_targetsDatabase.TryGetValue(collider, out target) && target != null)
        {
            return true;
        }

        Character character = collider.GetComponent<Character>();
        if (character != null)
        {
            target = character.FindAbility<CharacterAttack>();
            _targetsDatabase.Add(collider, target);   
        }
            
        return target != null;
    }

    private bool CheckLimits(CharacterAttack target)
    {
        Vector3 attackerToVictim = target.transform.position - _character.transform.position;

        Vector3 victimDirection = target.CharacterModel.TransformDirection(config.VictimDirectionLimit.LimitDirection);
        bool victimDirectionLimit = CheckLimit(config.VictimDirectionLimit, victimDirection, attackerToVictim);

        Vector3 attackerDirection = CharacterModel.TransformDirection(config.AttackerDirectionLimit.LimitDirection);
        bool attackerDirectionLimit = CheckLimit(config.AttackerDirectionLimit, attackerDirection, attackerToVictim);

        return victimDirectionLimit && attackerDirectionLimit;
    }
    
    private bool CheckLimit(CharacterAttackConfig.Limit limit, Vector3 a, Vector3 b)
    {
        if (!limit.UseLimit) return true;

        float angle = Vector3.Angle(a, b);
        return angle <= (limit.LimitAngle * 0.5f);
    }

    private void ApplyAnimation(AnimatorOverrideController controller, string key)
    {
        _character.CharacterAnimator.runtimeAnimatorController = controller ? controller : _defaultAnimatorController;
        if (controller != null)
        {
            _character.CharacterAnimator.SetTrigger(key);
        }
    }

    private void SendDamage()
    {
        if (_target != null)
        {
            _target.CharacterHealth.Damage(1, gameObject, 0f, 0f, Vector3.zero);
        }
    }

    private void CompleteAttack()
    {
        IsAttacking = false;
        _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
    }
}
