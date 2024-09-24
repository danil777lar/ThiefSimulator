using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuScreen : UIScreen
{
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button upgradesButton;
    [SerializeField] private GameObject upgradesAlarm;

    [InjectService] private UIService _uiService;
    [InjectService] private PlayerInputService _inputService;
    [InjectService] private UpgradesService _upgradesService;
    [InjectService] private ILevelManagerService _levelManagerService;

    protected override void OnBeforeOpen(UIObject.Args screenOpenProperties)
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        _inputService.EventPointerDown += OnPointerDown;

        shopButton.onClick.AddListener(OnShopButtonClicked);
        settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        upgradesButton.onClick.AddListener(OpenUpgradesPopup);
    }

    protected override void OnBeforeClose()
    {
        _inputService.EventPointerDown -= OnPointerDown;
    }

    private void Update()
    {
        upgradesAlarm.gameObject.SetActive(_upgradesService.CanMakeSomeUpgrade());
    }

    private void OnShopButtonClicked()
    {
        _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(new ShopScreen.Args());
    }
    
    private void OnSettingsButtonClicked()
    {
        _uiService.GetProcessor<UIPopupProcessor>().OpenPopup(new SettingsPopup.Args());
    }
    
    private void OpenUpgradesPopup()
    {
        _uiService.GetProcessor<UIPopupProcessor>().OpenPopup(new UpgradesPopup.Args());
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
