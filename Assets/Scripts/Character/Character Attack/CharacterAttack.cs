using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using DG.Tweening;
using MoreMountains.TopDownEngine;
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
    private CharacterController _characterController;
    private Character _target;
    private RuntimeAnimatorController _defaultAnimatorController;

    private Dictionary<Collider, CharacterAttack> _targetsDatabase = new Dictionary<Collider, CharacterAttack>();
    private Dictionary<CharacterAttack, AttackMarker> _markers = new Dictionary<CharacterAttack, AttackMarker>();

    public bool IsAttacking { get; private set; }

    public override void ProcessAbility()
    {
        base.ProcessAbility();

        if (!AbilityAuthorized || !AbilityPermitted)
        {
            return;
        }

        UpdateMarkers();
        TryFindTarget();
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

        _characterController = _character.GetComponent<CharacterController>();
        _defaultAnimatorController = _character.CharacterAnimator.runtimeAnimatorController;

        AnimatorEventReceiver receiver = _character.CharacterAnimator.GetComponent<AnimatorEventReceiver>();
        receiver.EventSendDamage += SendDamage;
        receiver.EventAttackComplete += CompleteAttack;
    }
    
    private void TryFindTarget()
    {
        if (!IsAttacking)
        {
            Vector3 origin = _characterController.transform.position +
                             Vector3.up * (_characterController.height * 0.5f);
            Vector3 direction = _character.CharacterModel.transform.forward;
            Ray ray = new Ray(origin, direction);

            float radius = _characterController.radius;
            if (Physics.SphereCast(ray, radius, out RaycastHit hit, config.AttackDistance, targetMask))
            {
                if (_target == null || _target.gameObject != hit.collider.gameObject)
                {
                    _target = hit.collider.GetComponent<Character>();
                }
            }
            else
            {
                _target = null;
            }
        }
    }

    private void TryStartAttack()
    {
        bool canAttack = _target != null && !IsAttacking && CheckLimits(); 
        
        if (canAttack)
        {
            CharacterAttack targetAttack = _target.FindAbility<CharacterAttack>();
            if (targetAttack != null && !targetAttack.IsAttacking)
            {
                targetAttack.Froze();
                IsAttacking = true;
                _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Frozen);

                TryStartTransition(() =>
                {
                    targetAttack.ApplyVictimAnimation(config.Animations);
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
                AttackMarker marker = Instantiate(markerPrefab, target.transform);
                _markers.Add(target, marker);
            }
        }
    }

    private List<CharacterAttack> FindTargetsInDistance(float distance)
    {
        List<CharacterAttack> result = new List<CharacterAttack>();
        Physics.OverlapSphere(transform.position, markerSpawnDistance, targetMask)
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

    private bool CheckLimits()
    {
        if (_target != null)
        {
            Vector3 playerToEnemy = _target.transform.position - _character.transform.position;
            
            Vector3 enemyDirection = _target.CharacterModel.transform.TransformDirection(config.EnemyDirectionLimit.LimitDirection);
            bool enemyDirectionLimit = CheckLimit(config.EnemyDirectionLimit, enemyDirection, playerToEnemy);
            
            Vector3 playerDirection = _character.CharacterModel.transform.TransformDirection(config.PlayerDirectionLimit.LimitDirection);
            bool playerDirectionLimit = CheckLimit(config.PlayerDirectionLimit, playerDirection, playerToEnemy);     
            
            return enemyDirectionLimit && playerDirectionLimit;
        }

        return false;
    }
    
    private bool CheckLimit(CharacterAttackConfig.Limit limit, Vector3 a, Vector3 b)
    {
        if (!limit.UseLimit) return true;

        if (_target != null)
        {
            float angle = Vector3.Angle(a, b);
            return angle <= limit.LimitAngle;
        }

        return false;
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
