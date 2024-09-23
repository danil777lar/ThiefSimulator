using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class ThiefCharacterRagdollOnDeath : CharacterRagdollOnDeath
{
    private List<Rigidbody> _rigidbodies;
    
    protected override void OnDeath()
    {
        base.OnDeath();
        GetRigidbodies().ForEach(x =>
        {
            x.linearVelocity = _health.LastDamageDirection;
        });
    }

    private List<Rigidbody> GetRigidbodies()
    {
        return _rigidbodies ??= Ragdoller.MainRigidbody.GetComponentsInChildren<Rigidbody>().ToList();
    }
}
