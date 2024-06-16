using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RevivePopup : UIPopup
{
    [Space(40f)]
    [SerializeField] private int delay; 
    [Header("Buttons")]
    [SerializeField] private Button reviveButton;
    [SerializeField] private Button skipButton;
    [Header("Heart")] 
    [SerializeField] private Image heartImage;
    [SerializeField] private TextMeshProUGUI heartTimer;
    
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
        
        StartCoroutine(HeartAnimationCoroutine());
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

    private IEnumerator HeartAnimationCoroutine()
    {
        skipButton.gameObject.SetActive(false);
        heartTimer.text = delay.ToString();
        
        for (int i = delay - 1; i >= 0; i--)
        {
            int time = i;
            heartImage.transform.DOScale(1.25f, 0.25f)
                .SetUpdate(UpdateType.Normal, true)
                .OnComplete(() =>
                {
                    heartTimer.text = time > 0 ? time.ToString() : "";
                    heartImage.transform.DOScale(1f, 0.25f)
                        .SetUpdate(UpdateType.Normal, true);

                    if (time == 0)
                    {
                        skipButton.gameObject.SetActive(true);
                    }
                });
            yield return new WaitForSecondsRealtime(1);
        }
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
