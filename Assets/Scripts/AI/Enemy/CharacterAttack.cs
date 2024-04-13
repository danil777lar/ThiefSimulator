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
        if (_target != null && !IsAttacking)
        {
            _character.CharacterAnimator.SetTrigger("Ram");
            _target.CharacterHealth.Damage(1, gameObject, 0f, 0f, Vector3.zero);
            StartCoroutine(AttackCooldown());   
        }
    }

    private IEnumerator AttackCooldown()
    {
        IsAttacking = true;
        yield return new WaitForSeconds(config.AttackCooldown);
        IsAttacking = false;
    }
}
