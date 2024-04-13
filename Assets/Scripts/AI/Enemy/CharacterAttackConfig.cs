using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Attack Config", menuName = "Configs/Character Attack")]
public class CharacterAttackConfig : ScriptableObject
{
    [field: SerializeField] public float AttackDistance { get; private set; } = 2f;
    [field: SerializeField] public float AttackCooldown { get; private set; } = 2f;
    [field: SerializeField] public float Damage { get; private set; } = 1f;

    [field: Header("Direction Limit")]
    [field: SerializeField] public bool UseLimit { get; private set; }
    [field: SerializeField] public float LimitAngle { get; private set; } = 20f;
    [field: SerializeField] public Vector3 LimitDirection { get; private set; } = Vector3.forward;
}
