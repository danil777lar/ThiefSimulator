using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Attention Config", menuName = "Configs/Enemy/Enemy Attention")]
public class EnemyAttentionConfig : ScriptableObject
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
    
    [field: Header("Seek")]
    [field: SerializeField] public float SeekPointDistance { get; private set; }
    
}
