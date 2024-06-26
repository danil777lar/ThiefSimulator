using System.Collections;
using System.Collections.Generic;
using Larje.Core.Tools.TopDownEngine;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class UpgradeFasterMovement : UpgradeProcessor
{
    private CoreCharacterMovement _movement;
    
    public override void Init(int level)
    {
        base.Init(level);

        _movement = GetComponentInParent<Character>().FindAbility<CoreCharacterMovement>();
        _movement.TryAddSpeedMultiplier(GetMultiplier);
    }
    
    public override void Remove()
    {
        base.Remove();
        _movement.TryRemoveSpeedMultiplier(GetMultiplier);
    }
    
    private float GetMultiplier()
    {
        return 1f + GetValueOnLevel(_currentLevel);
    }
}
