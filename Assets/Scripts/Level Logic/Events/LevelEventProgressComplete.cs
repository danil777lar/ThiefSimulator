using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEventProgressComplete : LevelEvent
{
    public readonly ProgressType Type;
    
    public LevelEventProgressComplete(ProgressType progressType)
    {
        Type = progressType;
    }
    
    public enum ProgressType
    {
        Min,
        Full
    }
}
