using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class UpgradeMoreWeigth : UpgradeProcessor
{
    private CharacterCarry3D _carry;

    public override void Init(int level)
    {
        base.Init(level);
        
        _carry = GetComponentInParent<Character>().FindAbility<CharacterCarry3D>();
        _carry.TryAddWeightCapacityMultiplier(GetMultiplier);
    }

    public override void Remove()
    {
        base.Remove();
        _carry.TryRemoveWeightCapacityMultiplier(GetMultiplier);
    }

    private float GetMultiplier()
    {
        return 1f + GetValueOnLevel(_currentLevel);
    }
}
