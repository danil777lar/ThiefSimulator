using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class AIDecisionPlayerInVision : AIDecision
{
    private CharacterSeek _seek;
    
    public override void Initialization()
    {
        base.Initialization();
        _seek = _brain.Owner.GetComponent<CharacterSeek>();
    }

    public override bool Decide()
    {
        return _seek.PlayerInVision;
    }
}
