using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using UnityEngine;

public class ThiefLevel : LevelProcessor
{
    public override void TryStartLevel(StartData data)
    {
        StartLevel(data);
    }

    public override void TryStopLevel(StopData data)
    {
        StopLevel(data);
    }

    public override LevelData GetLevelData()
    {
        return null;
    }
}
