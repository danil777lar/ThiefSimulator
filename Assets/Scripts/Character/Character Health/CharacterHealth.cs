using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterHealth : Health
{
    [MMInspectorGroup("Thief", true, 6)]
    [SerializeField] private bool save;

    public event Action EventDamageDeclined;

    public override void Damage(float damage, GameObject instigator, float flickerDuration, float invincibilityDuration,
        Vector3 damageDirection, List<TypedDamage> typedDamages = null)
    {
        if (save)
        {
            EventDamageDeclined?.Invoke();
            return;
        }
        
        base.Damage(damage, instigator, flickerDuration, invincibilityDuration, damageDirection, typedDamages);
    }
}
