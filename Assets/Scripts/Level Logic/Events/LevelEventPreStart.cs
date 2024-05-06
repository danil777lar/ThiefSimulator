using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEventPreStart : LevelEvent
{
    public readonly float StartDelay;
    
    public LevelEventPreStart(float startDelay)
    {
        StartDelay = startDelay;
    }
}
