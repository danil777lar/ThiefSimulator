using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using ProjectConstants;
using UnityEngine;
using UnityEngine.Analytics;

public class ThiefLevelAnaliticsMediator : MonoBehaviour, ILevelStartHandler, ILevelEndHandler
{
    [InjectService] private ILevelManagerService _levelService;
    [InjectService] private IAnalyticsService _analyticsService;
    
    public void OnLevelStarted(LevelProcessor.StartData data)
    {
        if (data.StartType == LevelStartType.Start)
        {
            SendEvent("Start");
        }
    }

    public void OnLevelEnded(LevelProcessor.StopData data)
    {
        if (data.StopType == LevelStopType.Fail)
        {
            SendEvent("Fail");
        }
        else if (data.StopType == LevelStopType.Win)
        {
            SendEvent("Win");
        }
    }

    private void Start()
    {
        DIContainer.InjectTo(this);
    }
    
    private void SendEvent(string eventName)
    {
        int levelIndex = _levelService.GetCurrentLevelIndex();
        string fullEvent = $"Level_{levelIndex}_{eventName}";

        _analyticsService.SendEvent(fullEvent);
    }
}
