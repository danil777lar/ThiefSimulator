using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Seek Config", menuName = "Configs/Character Seek Config")]
public class CharacterSeekConfig : ScriptableObject
{
    [field: SerializeField] public float SeekDistance { get; private set; }
    [field: Header("Speed")]
    [field: SerializeField] public float PatrolSpeed { get; private set; }
    [field: SerializeField] public float AttackSpeed { get; private set; }
    [field: SerializeField] public float RunToSeekPointSpeed { get; private set; }
    [field: SerializeField] public float SeekSpeed { get; private set; }
}
