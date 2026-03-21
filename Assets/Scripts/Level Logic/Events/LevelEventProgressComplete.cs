using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Larje.Core;

public class LevelEventProgressComplete : GameEvent
{
    public readonly ProgressType Type;

    public override bool IsValid => true;
    
    public LevelEventProgressComplete(ProgressType progressType) : base("")
    {
        Type = progressType;
    }
    
    public enum ProgressType
    {
        Min,
        Full
    }
}
