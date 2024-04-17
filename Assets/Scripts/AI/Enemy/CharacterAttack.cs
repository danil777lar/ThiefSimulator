using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using DG.Tweening;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterAttack : CharacterAbility
{
    [SerializeField] private CharacterAttackConfig config;
    [SerializeField] private LayerMask targetMask;

    private bool _grabbed;
    private CharacterController _characterController;
    private Character _target;
    private RuntimeAnimatorController _defaultAnimatorController;

    public bool IsAttacking { get; private set; }

    public override void ProcessAbility()
    {
        base.ProcessAbility();

        if (!AbilityAuthorized || !AbilityPermitted)
        {
            return;
        }

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
        if (_target != null && !IsAttacking && CheckLimit())
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

    private bool CheckLimit()
    {
        if (!config.UseLimit) return true;

        if (_target != null)
        {
            Vector3 directionSelf = _character.CharacterModel.transform.forward;
            Vector3 directionTarget = _target.CharacterModel.transform.TransformDirection(config.LimitDirection);

            Debug.DrawRay(transform.position, directionSelf * 5f, Color.blue);
            Debug.DrawRay(_target.transform.position, directionTarget * 5f, Color.red);

            float angle = Vector3.Angle(directionSelf, directionTarget);
            return angle <= config.LimitAngle;
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
