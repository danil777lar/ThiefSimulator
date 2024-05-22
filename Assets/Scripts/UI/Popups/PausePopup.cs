using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.UI;

public class PausePopup : UIPopup
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [InjectService] private UIService _uiService;
    [InjectService] private ILevelManagerService _levelManagerService;
    
    protected override void OnBeforeOpen(UIObject.Args args)
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        
        resumeButton.onClick.AddListener(OnResumeButtonClicked);
        restartButton.onClick.AddListener(OnRestartButtonClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
    }

    private void OnResumeButtonClicked()
    {
        Close();
    }

    private void OnRestartButtonClicked()
    {
        Close();
        
        _levelManagerService.SpawnCurrentLevel();
        _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(new LoadingScreen.Args(true,
            () =>
            {
                //_uiService.GetProcessor<UIScreenProcessor>().OpenScreen(new LoadingScreen.Args();
                _levelManagerService.TryStartCurrentLevel(new LevelProcessor.StartData(LevelStartType.Start));
            }));
    }

    private void OnMainMenuButtonClicked()
    {
        Close();
        
        _levelManagerService.SpawnCurrentLevel();
        _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(new LoadingScreen.Args(true, null));
    }

    public new class Args : UIPopup.Args
    {
        public Args() : base(UIPopupType.Pause)
        {
        }

        public Args(UIPopupCombinationType combinationType) : base(UIPopupType.Pause, combinationType)
        {
        }
    }
}
