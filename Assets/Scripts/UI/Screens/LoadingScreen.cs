using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : UIScreen
{
        [SerializeField] private float usualLoadingDuration;
        [SerializeField] private float firstStartLoadingDuration;
        [SerializeField] private float levelLoadingDuration;
        [Space] 
        [SerializeField] private Slider loadingProgress;
        [SerializeField] private TextMeshProUGUI loadingPercent;

        [InjectService] private ILevelManagerService _levelService;
        [InjectService] private IAdsService _adsService;
        [InjectService] private UIService _uiService;
        [InjectService] private IDataService _dataService;

        private void Awake()
        {
            DIContainer.InjectTo(this);
        }

        protected override void OnBeforeOpen(UIObject.Args args)
        {
            bool showInter = _dataService.SystemData.IternalData.SessionNum != 1;
            float loadingDuration = _dataService.SystemData.IternalData.SessionNum == 1
                ? firstStartLoadingDuration
                : usualLoadingDuration;
            Action onLoadComplete = OnLoadingCompleteDefault;
            
            if (args is Args loadingArgs)
            {
                showInter = loadingArgs.ShowInter;
                if (loadingArgs.OnLoadComplete != null)
                {
                    onLoadComplete = loadingArgs.OnLoadComplete;
                }
                loadingDuration = levelLoadingDuration;
            }

            DOTween.To(() => 0,
                    (x) =>
                    {
                        loadingProgress.value = x;
                        loadingPercent.text = $"{Mathf.Floor(Mathf.Lerp(0, 100, x))}%";
                    },
                    1f, loadingDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    onLoadComplete?.Invoke();
                    if (showInter)
                    {
                        _adsService.ShowInterstitial();
                    }
                });
        }

        private void OnLoadingCompleteDefault()
        {
            _uiService.GetProcessor<UIScreenProcessor>()
                .OpenScreen(new MenuScreen.Args());
        }

        public class Args : UIScreen.Args
        {
            public readonly bool ShowInter;
            public readonly Action OnLoadComplete;

            public Args(bool showInter, Action onLoadComplete) : base(UIScreenType.Loading)
            {
                ShowInter = showInter;
                OnLoadComplete = onLoadComplete;
            }
        }
}
