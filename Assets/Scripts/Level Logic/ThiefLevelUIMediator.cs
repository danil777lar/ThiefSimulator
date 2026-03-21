using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;

public class ThiefLevelUIMediator : MonoBehaviour
{
    [InjectService] private UIService _uiService;
    
    private void Start()
    {
        DIContainer.InjectTo(this);
    }

    public void OnLevelEvent()
    {
        // if (levelEvent is LevelEventPreStart)
        // {
        //     _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(new PlayScreen.Args());    
        // }
    }

    public void OnLevelStarted(LevelProcessor.StartData data)
    { 
        
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
