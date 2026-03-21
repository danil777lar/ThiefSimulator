using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using ProjectConstants;
using UnityEngine;
using Unity.Cinemachine;

public class LevelCameraActivator : MonoBehaviour, ILevelStartHandler, ILevelEndHandler
{
    [SerializeField] private List<StartFeedback> startFeedbacks;
    [SerializeField] private List<StopFeedback> stopFeedbacks;
    
    private CinemachineVirtualCamera _vCam;

    private void Start()
    {
        _vCam = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    public void OnLevelStarted(LevelProcessor.StartData data)
    {
        if (startFeedbacks == null)
        {
            return;
        }

        StartFeedback feedback = startFeedbacks.Find(x => x.StartType == data.StartType);
        if (feedback != null)
        {
            _vCam.Priority = feedback.Priority;
        }
    }
    
    public void OnLevelEnded(LevelProcessor.StopData data)
    {
        if (stopFeedbacks == null)
        {
            return;
        }

        StopFeedback feedback = stopFeedbacks.Find(x => x.StopType == data.StopType);
        if (feedback != null)
        {
            _vCam.Priority = feedback.Priority;
        }
    }

    [Serializable]
    public class StartFeedback
    {
        [field: SerializeField] public LevelStartType StartType;
        [field: SerializeField] public int Priority;
    }
    
    [Serializable]
    public class StopFeedback
    {
        [field: SerializeField] public LevelStopType StopType;
        [field: SerializeField] public int Priority;
    }
}
