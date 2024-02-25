using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuScreen : UIScreen
{
    [SerializeField] private RectTransformEvents rect;

    [InjectService] private ILevelManagerService _levelManagerService;
    [InjectService] private UIService _uiService;

    protected override void OnOpen(ScreenOpenProperties screenOpenProperties)
    {
        base.OnOpen(screenOpenProperties);
        ServiceLocator.Default.InjectServicesInComponent(this);
        rect.EventPointerDown += OnPointerDown;
    }

    private void OnPointerDown(PointerEventData data)
    {
        _levelManagerService.TryStartCurrentLevel(new LevelProcessor.StartData(LevelStartType.Start));
    }

    public class Args : ScreenOpenProperties
    {
        public Args() : base(UIScreenType.Menu)
        {
            
        }
    }
}
