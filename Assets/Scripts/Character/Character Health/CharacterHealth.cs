using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using ProjectConstants;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterHealth : Health
{
    [MMInspectorGroup("Thief", true, 6)]
    [SerializeField, Min(0)] private int maxDamageDeclines = 1;

    private int currentDamageDeclines;
    
    public event Action EventDamageDeclined;

    public override void Damage(float damage, GameObject instigator, float flickerDuration, float invincibilityDuration,
        Vector3 damageDirection, List<TypedDamage> typedDamages = null)
    {
        if (_character.CharacterType == Character.CharacterTypes.Player && currentDamageDeclines < maxDamageDeclines)
        {
            ServiceLocator.Instance.GetService<UpgradesService>()
                .SpawnUpgrade(UpgradeType.Invisibility, 0, transform);
            
            currentDamageDeclines++;
            EventDamageDeclined?.Invoke();
            return;
        }
        
        base.Damage(damage, instigator, flickerDuration, invincibilityDuration, damageDirection, typedDamages);
    }
}
