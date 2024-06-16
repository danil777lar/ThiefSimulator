using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    [InjectService] private TimeScaleService _timeScaleService;

    private bool _interactable = false;
    private bool _closable = false;

    public override void Close()
    {
        if (!_closable) return;
        
        base.Close();
    }

    protected override void OnBeforeOpen(UIObject.Args args)
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        
        resumeButton.onClick.AddListener(OnResumeButtonClicked);
        restartButton.onClick.AddListener(OnRestartButtonClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        
        _timeScaleService.PlayTimeScaleAnim(TimeScaleAnimationType.Stop);
        
        DOVirtual.DelayedCall(0.25f, () =>
        {
            _interactable = true;
            _closable = true;
        });
    }

    protected override void OnBeforeClose()
    {
        _timeScaleService.PlayTimeScaleAnim(TimeScaleAnimationType.Start);
    }

    private void OnResumeButtonClicked()
    {
        if (!_interactable) return;
        _interactable = false;
        
        Close();
    }

    private void OnRestartButtonClicked()
    {
        if (!_interactable) return;
        _interactable = false;
        
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
        if (!_interactable) return;
        _interactable = false;
        
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
