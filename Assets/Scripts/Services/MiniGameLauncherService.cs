using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;

[BindService(typeof(MiniGameLauncherService))]
public class MiniGameLauncherService : Service
{
    [SerializeField] private List<MiniGamePopupOption> miniGamePopupOptions;
    
    [InjectService] private UIService _uiService;
    
    public override void Init()
    {
        
    }

    public void LaunchMiniGame(MiniGameType miniGameType, float multiplier, Action onComplete, Action onFail)
    {
        MiniGamePopup.MiniGameArgs popup = new MiniGamePopup.MiniGameArgs(
            GetPopupType(miniGameType), 
            multiplier,
            () => onComplete.Invoke(),
            () => onFail.Invoke());
        
        _uiService.GetProcessor<UIPopupProcessor>().OpenPopup(popup);
    }

    private UIPopupType GetPopupType(MiniGameType type)
    {
        return miniGamePopupOptions.Find(x => x.MiniGameType == type).PopupType;
    }

    [Serializable]
    private class MiniGamePopupOption
    {
        [field: SerializeField] public MiniGameType MiniGameType { get; private set; }
        [field: SerializeField] public UIPopupType PopupType { get; private set; }
    }
}
