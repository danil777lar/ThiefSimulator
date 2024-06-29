using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using ProjectConstants;
using UnityEngine;
using UnityEngine.Analytics;

public class ThiefLevelAnaliticsMediator : MonoBehaviour, ILevelStartHandler, ILevelEndHandler
{
    [InjectService] private ILevelManagerService _levelService;
    
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
        ServiceLocator.Instance.InjectServicesInComponent(this);
    }
    
    private void SendEvent(string eventName)
    {
        int levelIndex = _levelService.GetCurrentLevelIndex();
        string fullEvent = $"Level_{levelIndex}_{eventName}";

        Analytics.CustomEvent(fullEvent);
        Debug.Log($"<color=yellow>{fullEvent}</color>");   
    }
}
