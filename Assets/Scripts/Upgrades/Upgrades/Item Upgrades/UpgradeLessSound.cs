using System.Collections;
using System.Collections.Generic;
using Larje.Character;
using UnityEngine;

public class UpgradeLessSound : UpgradeProcessor
{
    private CharacterStepSound _stepSound;
    
    public override void Init(int level)
    {
        base.Init(level);
        
        _stepSound = GetComponentInParent<Character>().GetComponentInChildren<CharacterStepSound>();
        _stepSound.TryAddAmplitudeMultiplier(GetMultiplier);
    }

    public override void Remove()
    {
        base.Remove();
        _stepSound.TryRemoveAmplitudeMultiplier(GetMultiplier);
    }

    private float GetMultiplier()
    {
        return 1f - Mathf.Min(1f, GetValueOnLevel(_currentLevel));
    }
}
