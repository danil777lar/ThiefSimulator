using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterAttack : CharacterAbility
{
    [SerializeField] private CharacterAttackConfig config;
    [SerializeField] private LayerMask targetMask;

    private CharacterController _characterController;
    private Character _target;

    public bool IsAttacking { get; private set; }

    protected override void Initialization()
    {
        base.Initialization();

        _characterController = GetComponent<CharacterController>();
    }

    public override void ProcessAbility()
    {
        base.ProcessAbility();

        if (!AbilityAuthorized || !AbilityPermitted)
        {
            return;
        }
        
        TryFindTarget();
        TrySendDamage();
    }

    private void TryFindTarget()
    {
        Vector3 origin = _characterController.transform.position + Vector3.up * (_characterController.height * 0.5f);
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

    private void TrySendDamage()
    {
        if (_target != null && !IsAttacking && CheckLimit())
        {
            _character.CharacterAnimator.SetTrigger("Ram");
            _target.CharacterHealth.Damage(1, gameObject, 0f, 0f, Vector3.zero);
            StartCoroutine(AttackCooldown());   
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

    private IEnumerator AttackCooldown()
    {
        IsAttacking = true;
        yield return new WaitForSeconds(config.AttackCooldown);
        IsAttacking = false;
    }
}
