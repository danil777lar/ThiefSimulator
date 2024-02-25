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

    protected override void OnOpen(ScreenOpenProperties screenOpenProperties)
    {
        base.OnOpen(screenOpenProperties);
        ServiceLocator.Default.InjectServicesInComponent(this);

        skipButton.onClick.AddListener(OnSkipButtonClicked);
    }

    private void OnSkipButtonClicked()
    {
        _uiService.Screens.OpenScreen(new LoadingScreen.Args(false,null));
    }

    public class Args : ScreenOpenProperties
    {
        public Args() : base(UIScreenType.Win)
        {
            
        }
    }
}
