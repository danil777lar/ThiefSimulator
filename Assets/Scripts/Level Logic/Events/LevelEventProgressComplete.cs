using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Larje.Core;

public class LevelEventProgressComplete : GameEvent
{
    public override bool IsValid => true;
    
    public LevelEventProgressComplete() : base("")
    {
    }
}
