using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class AIDecisionIsAttack : AIDecision
{
    private CharacterSeek _characterSeek;

    public override void Initialization()
    {
        base.Initialization();
        _characterSeek = _brain.Owner.GetComponent<CharacterSeek>();
    }

    public override bool Decide()
    {
        if (_characterSeek != null)
        {
            return _characterSeek.IsAttack; 
        }
        return false;
    }
}
