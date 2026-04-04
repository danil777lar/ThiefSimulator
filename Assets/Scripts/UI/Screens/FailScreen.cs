using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.UI;

public class FailScreen : UIScreen
{
    [SerializeField] private Button retryButton;

    [InjectService] private ILevelManagerService _levelService;
    [InjectService] private UIService _uiService;

    private Args _args;

    protected override void OnBeforeOpen(UIObject.Args args)
    {
        DIContainer.InjectTo(this);

        if (args is Args failArgs)
        {
            _args = failArgs;
        }

        retryButton.onClick.AddListener(OnRetryButtonClicked);
    }

    private void OnRetryButtonClicked()
    {
        _args.OnNext?.Invoke();
    }
    
    public class Args : UIScreen.Args
    {
        public readonly Action OnNext;

        public Args(Action onNext) : base(UIScreenType.Fail)
        {
            OnNext = onNext;
        }
    }
}
