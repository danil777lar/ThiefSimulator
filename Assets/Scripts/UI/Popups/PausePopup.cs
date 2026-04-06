using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Larje.Core;
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
    [InjectService] private ThiefGameService _thiefGameService;

    private bool _interactable = false;
    private bool _closable = false;

    public override void Close()
    {
        if (!_closable) return;
        
        base.Close();
    }

    protected override void OnBeforeOpen(UIObject.Args args)
    {
        DIContainer.InjectTo(this);
        
        resumeButton.onClick.AddListener(OnResumeButtonClicked);
        restartButton.onClick.AddListener(OnRestartButtonClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        
        DOVirtual.DelayedCall(0.25f, () =>
        {
            _interactable = true;
            _closable = true;
        });
    }

    private void OnResumeButtonClicked()
    {
        if (!_interactable) return;
        _interactable = false;
        
        Close();
    }

    private void OnRestartButtonClicked()
    {
        _thiefGameService.RestartLevel();
        Close();
    }

    private void OnMainMenuButtonClicked()
    {
        _thiefGameService.ReturnToMenu();
        Close();
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
