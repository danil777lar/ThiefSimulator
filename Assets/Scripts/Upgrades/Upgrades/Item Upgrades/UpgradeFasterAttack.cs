using System.Collections;
using System.Collections.Generic;
using Larje.Character;
using UnityEngine;

public class UpgradeFasterAttack : UpgradeProcessor
{
    private CharacterAttack _attack; 
    
    public override void Init(int level)
    {
        base.Init(level);

        Character character = GetComponentInParent<Character>();
        _attack = character?.GetComponent<CharacterAttack>();
        if (_attack != null)
        {
            _attack.AddAttackSpeedModifier(GetModifier);
        }
    }

    public override void Remove()
    {
        if (_attack != null)
        {
            _attack.RemoveAttackSpeedModifier(GetModifier);
        }
        
        base.Remove();
    }
    
    private float GetModifier()
    {
        return GetValueOnLevel(_currentLevel);
    }
}
