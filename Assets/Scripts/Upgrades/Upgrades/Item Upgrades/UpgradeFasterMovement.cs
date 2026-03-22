using System.Collections;
using System.Collections.Generic;
using Larje.Character;
using Larje.Character.Abilities;
using UnityEngine;

public class UpgradeFasterMovement : UpgradeProcessor
{
    private CharacterWalk _movement;
    
    public override void Init(int level)
    {
        base.Init(level);

        _movement = GetComponentInParent<Character>().GetComponent<CharacterWalk>();
        _movement.WalkMultiplier.AddValue(GetMultiplier);
    }
    
    public override void Remove()
    {
        base.Remove();
        _movement.WalkMultiplier.RemoveValue(GetMultiplier);
    }
    
    private float GetMultiplier()
    {
        return 1f + GetValueOnLevel(_currentLevel);
    }
}
