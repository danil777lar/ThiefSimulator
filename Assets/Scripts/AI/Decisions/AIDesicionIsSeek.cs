using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class AIDesicionIsSeek : AIDecision
{
    private CharacterAttention _attention;
    
    public override void Initialization()
    {
        base.Initialization();
        _attention = _brain.Owner.GetComponent<CharacterAttention>();
    }

    public override bool Decide()
    {
        return _attention.IsSeek;
    }
}
