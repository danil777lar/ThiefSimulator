using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Seek Config", menuName = "Configs/Enemy/Enemy Seek")]
public class EnemySeekConfig : ScriptableObject
{
    [field: SerializeField] public float MaxSeekDistance { get; private set; }
    [field: Header("Points Observe")]
    [field: SerializeField] public float ForceObserveDistance { get; private set; }
    [field: SerializeField] public float MaxObserveDistance { get; private set; }
    [field: SerializeField] public float PointObserveSpeed { get; private set; }
    [field: SerializeField] public float PointRecoverySpeed { get; private set; }
}
