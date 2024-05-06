using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuScreen : UIScreen
{
    [InjectService] private ILevelManagerService _levelManagerService;
    [InjectService] private UIService _uiService;
    [InjectService] private PlayerInputService _inputService;

    protected override void OnBeforeOpen(UIObject.Args screenOpenProperties)
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        _inputService.EventPointerDown += OnPointerDown;
    }

    protected override void OnBeforeClose()
    {
        _inputService.EventPointerDown -= OnPointerDown;
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
