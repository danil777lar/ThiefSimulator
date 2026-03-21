using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Larje.Core;

public class LevelEventPreStart : GameEvent
{
    public readonly float StartDelay;

    public override bool IsValid => true;
    
    public LevelEventPreStart(float startDelay) : base("")
    {
        StartDelay = startDelay;
    }
}
