using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Attack Config", menuName = "Configs/Character Attack")]
public class CharacterAttackConfig : ScriptableObject
{
    [field: SerializeField] public float AttackDistance { get; private set; }
    [field: SerializeField] public float AttackCooldown { get; private set; }
}
