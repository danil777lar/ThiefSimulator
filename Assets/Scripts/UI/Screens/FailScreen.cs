using System.Collections;
using System.Collections.Generic;
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

    protected override void OnBeforeOpen(UIObject.Args args)
    {
        ServiceLocator.Default.InjectServicesInComponent(this);

        retryButton.onClick.AddListener(OnRetryButtonClicked);
    }

    private void OnRetryButtonClicked()
    {
        _levelService.SpawnCurrentLevel();
        _uiService.GetProcessor<UIScreenProcessor>()
            .OpenScreen(new LoadingScreen.Args(false, null));
    }
    
    public class Args : UIScreen.Args
    {
        public Args() : base(UIScreenType.Fail)
        {
            
        }
    }
}
