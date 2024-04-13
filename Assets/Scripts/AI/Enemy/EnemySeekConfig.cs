using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Seek Config", menuName = "Configs/Enemy/Enemy Seek")]
public class EnemySeekConfig : ScriptableObject
{
    [field: SerializeField] public float MaxSeekDistance { get; private set; }
}
