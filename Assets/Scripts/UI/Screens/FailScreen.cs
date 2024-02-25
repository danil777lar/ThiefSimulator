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

    protected override void OnOpen(ScreenOpenProperties screenOpenProperties)
    {
        base.OnOpen(screenOpenProperties);
        ServiceLocator.Default.InjectServicesInComponent(this);

        retryButton.onClick.AddListener(OnRetryButtonClicked);
    }

    private void OnRetryButtonClicked()
    {
        _uiService.Screens.OpenScreen(new LoadingScreen.Args(false, null));
    }
    
    public class Args : ScreenOpenProperties
    {
        public Args() : base(UIScreenType.Fail)
        {
            
        }
    }
}
