using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using ProjectConstants;
using UnityEngine;

public class PlayerLevelCamera : MonoBehaviour, ILevelEndHandler
{
    private LevelProcessor _level;
    
    public void OnLevelEnded(LevelProcessor.StopData data)
    {
        if (data.StopType == LevelStopType.Win)
        {
            transform.SetParent(_level.transform);
        }
    }

    private void Start()
    {
        _level = GetComponentInParent<LevelProcessor>();
    }
}
