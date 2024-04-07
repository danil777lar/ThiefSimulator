using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;

public class ThiefLevelUIMediator : MonoBehaviour, ILevelStartHandler, ILevelEndHandler
{
    [InjectService] private UIService _uiService;
    
    private void Start()
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
    }
    
    public void OnLevelStarted(LevelProcessor.StartData data)
    { 
        _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(new PlayScreen.Args());
    }

    public void OnLevelEnded(LevelProcessor.StopData data)
    {
        if (data.StopType == LevelStopType.Fail)
        {
            _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(new FailScreen.Args());
        }
        else
        {
            _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(new WinScreen.Args());
        }
    }
}
