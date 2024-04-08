using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Seek Config", menuName = "Configs/Character Seek Config")]
public class CharacterSeekConfig : ScriptableObject
{
    [field: Header("Suspicion")]
    [field: SerializeField] public float SuspicionDecreaseDelay { get; private set; }
    [field: SerializeField] public float SuspicionDecreaseSpeed { get; private set; }
    [field: SerializeField] public float MaxSuspicion { get; private set; }
    [field: Header("Aggression")]
    [field: SerializeField] public float AggressionDecreaseDelay { get; private set; }
    [field: SerializeField] public float AggressionDecreaseSpeed { get; private set; }
    [field: SerializeField] public float MaxAggression { get; private set; }
    [field: Header("Vision")]
    [field: SerializeField] public float VisionDistance { get; private set; }
    [field: SerializeField] public float VisionSensitivity { get; private set; }
    [field: Header("Hearing")]
    [field: SerializeField] public float HearingSensitivity { get; private set; }
    [field: Header("Attack")]
    [field: SerializeField] public float AttackDistance { get; private set; }
    [field: SerializeField] public float AttackCooldown { get; private set; }
    [field: Header("Speed")]
    [field: SerializeField] public float PatrolSpeed { get; private set; }
    [field: SerializeField] public float AttackSpeed { get; private set; }
    [field: SerializeField] public float RunToSeekPointSpeed { get; private set; }
    [field: SerializeField] public float SeekSpeed { get; private set; }
}
