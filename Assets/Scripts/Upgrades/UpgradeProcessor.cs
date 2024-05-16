using System.Collections;
using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;

public abstract class UpgradeProcessor : MonoBehaviour
{
    [field: SerializeField] public UpgradeType UpgradeType { get; private set; }
    [field: SerializeField] public bool Upgradable { get; private set; }
    [field: Space]
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public string DescriptionLeftModifier { get; private set; }
    [field: SerializeField] public string DescriptionRightModifier { get; private set; }
    [field: Space]
    [field: SerializeField] public float MaxLevel { get; private set; }
    [field: SerializeField] public float BaseValue { get; private set; }
    [field: SerializeField] public float AddValuePerLevel { get; private set; }
    
    public string GetDescription(int level)
    {
        //return $"{DescriptionLeftModifier}{GetValueOnLevel(level)}{DescriptionRightModifier}";
        return $"Level {level + 1}";
    }

    private float GetValueOnLevel(int level)
    {
        return BaseValue + (AddValuePerLevel * level);
    }
}
