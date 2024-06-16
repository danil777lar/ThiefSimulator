using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.UI;

public class RevivePopup : UIPopup
{
    [SerializeField] private Button reviveButton;
    [SerializeField] private Button skipButton;
    
    [InjectService] private IAdsService _adsService;
    [InjectService] private TimeScaleService _timeScaleService;

    private bool _interactable = true;
    private Args _args;
    
    protected override void OnBeforeOpen(UIObject.Args args)
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        
        if (args is Args reviveArgs)
        {
            _args = reviveArgs;
        }
        
        reviveButton.onClick.AddListener(OnReviveButtonClicked);
        skipButton.onClick.AddListener(OnSkipButtonClicked);
        
        _timeScaleService.PlayTimeScaleAnim(TimeScaleAnimationType.Stop);
    }
    
    protected override void OnBeforeClose()
    {
        _timeScaleService.PlayTimeScaleAnim(TimeScaleAnimationType.Start);
    }

    private void OnReviveButtonClicked()
    {
        if (!_interactable) return;
        _interactable = false;
        
        _adsService.ShowRewarded(null, null,
            () =>
            {
                _args.OnRevive?.Invoke();
                Close();
            },
            () =>
            {
                _interactable = true;
            });
    }
    
    private void OnSkipButtonClicked()
    {
        if (!_interactable) return;
        _interactable = false;
        
        _args.OnSkip?.Invoke();
        Close();
    }

    public new class Args : UIPopup.Args
    {
        public readonly Action OnRevive;
        public readonly Action OnSkip;
        
        public Args(Action onRevive, Action onSkip) : base(UIPopupType.Revive)
        {
            OnRevive = onRevive;
            OnSkip = onSkip;
        }

        public Args(Action onRevive, Action onSkip, UIPopupCombinationType combinationType) : base(UIPopupType.Revive, combinationType)
        {
            OnRevive = onRevive;
            OnSkip = onSkip;
        }
    }
}
