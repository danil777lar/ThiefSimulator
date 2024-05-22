using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.UI;

public class PausePopup : UIPopup
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    
    protected override void OnBeforeOpen(UIObject.Args args)
    {
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
    }

    private void OnMainMenuButtonClicked()
    {
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
