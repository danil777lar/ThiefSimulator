using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuScreen : UIScreen
{
    [SerializeField] private Button shopButton; 
    [SerializeField] private Button settingsButton;

    [InjectService] private UIService _uiService;
    [InjectService] private PlayerInputService _inputService;
    [InjectService] private ILevelManagerService _levelManagerService;

    protected override void OnBeforeOpen(UIObject.Args screenOpenProperties)
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        _inputService.EventPointerDown += OnPointerDown;

        shopButton.onClick.AddListener(OnShopButtonClicked);
        settingsButton.onClick.AddListener(OnSettingsButtonClicked);
    }

    protected override void OnBeforeClose()
    {
        _inputService.EventPointerDown -= OnPointerDown;
    }

    private void OnShopButtonClicked()
    {
        _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(new ShopScreen.Args());
    }
    
    private void OnSettingsButtonClicked()
    {
        _uiService.GetProcessor<UIPopupProcessor>().OpenPopup(new SettingsPopup.Args());
    }
    
    private void OnPointerDown()
    {
        _levelManagerService.TryStartCurrentLevel(new LevelProcessor.StartData(LevelStartType.Start));
    }

    public class Args : UIScreen.Args
    {
        public Args() : base(UIScreenType.Menu)
        {
            
        }
    }
}
