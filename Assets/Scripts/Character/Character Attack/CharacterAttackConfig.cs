using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Attack Config", menuName = "Configs/Character Attack")]
public class CharacterAttackConfig : ScriptableObject
{
    [field: SerializeField] public float AttackDistance { get; private set; } = 2f;
    [field: SerializeField] public float AttackSpeed { get; private set; } = 0.5f;
    [field: SerializeField] public float Damage { get; private set; } = 1f;
    
    [field: Space]
    [field: SerializeField] public AnimatorOverrideController Animations { get; private set; }

    [field: Header("Limits")]
    [field: SerializeField] public Limit AttackerDirectionLimit { get; private set; }
    [field: SerializeField] public Limit VictimDirectionLimit { get; private set; }

    [field: Header("Attack Duration")]
    [field: SerializeField] public bool UseConstantAttackDuration { get; private set; }
    [field: SerializeField] public float ConstantAttackDuration { get; private set; } = 0.5f;
    
    [field: Header("Attack Ram")]
    [field: SerializeField] public float AttackRamDistance { get; private set; }
    [field: SerializeField] public float AttackRamDuration { get; private set; }
    [field: SerializeField] public Ease AttackRamEase { get; private set; }
    
    [field: Header("Fixed Position")]
    [field: SerializeField] public bool UseFixedPosition { get; private set; }

    [field: SerializeField] public float TransitionToFixedPositionDuration { get; private set; }
    [field: SerializeField] public Vector3 FixedPosition { get; private set; }

    
    [Serializable]
    public class Limit
    {
        [field: SerializeField] public bool UseLimit { get; private set; }
        [field: SerializeField] public float LimitAngle { get; private set; } = 20f;
        [field: SerializeField] public Vector3 LimitDirection { get; private set; } = Vector3.forward;    
    }
}
