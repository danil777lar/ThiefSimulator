using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Carryable Config", menuName = "Configs/Carryable Config")]
public class CarryableConfig : ScriptableObject
{
    [field: SerializeField] public float DropForce { get; private set; }

    [field: Header("Bend")]
    [field: SerializeField] public float MaxRotate { get; private set; } = 5f;

    [field: SerializeField] public float ForceMultiplier { get; private set; } = 5f;
    [field: SerializeField] public float SpringForce { get; private set; } = 1f;
    [field: SerializeField] public float SpringDrag { get; private set; } = 1f;
    
    [field: Header("Anchoring Animation")] 
    [field: SerializeField] public float AnchoringDuration { get; private set; } = 0.5f;
    [field: SerializeField] public float AnchoringTrajectoryHeight { get; private set; } = 1f;
}
