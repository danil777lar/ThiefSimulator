using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : UIScreen
{
    [SerializeField] private Button skipButton;

    [InjectService] private ILevelManagerService _levelService;
    [InjectService] private UIService _uiService;

    protected override void OnBeforeOpen(UIObject.Args args)
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        skipButton.onClick.AddListener(OnSkipButtonClicked);
    }

    private void OnSkipButtonClicked()
    {
        _levelService.IncrementLevelId();
        _levelService.SpawnCurrentLevel();
        _uiService.GetProcessor<UIScreenProcessor>()
            .OpenScreen(new LoadingScreen.Args(false,null));
    }

    public class Args : UIScreen.Args
    {
        public Args() : base(UIScreenType.Win)
        {
            
        }
    }
}
