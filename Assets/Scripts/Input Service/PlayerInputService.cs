using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using ProjectConstants;
using UnityEngine;

[BindService(typeof(PlayerInputService))]
public class PlayerInputService : Service
{
    [SerializeField] private UIScreenTypes screensMask; 
    
    [InjectService] private UIService _uiService;
    
    private InputManager _inputManager;
    private MMTouchJoystick _joystick;

    public event Action EventPointerDown; 
    
    public override void Init()
    {
        _inputManager = GetComponent<InputManager>();
        _joystick = GetComponentInChildren<MMTouchJoystick>(true);
        _uiService.GetProcessor<UIScreenProcessor>().ScreenChanged += OnScreenChanged;
    }

    public void PointerDown()
    {
        EventPointerDown?.Invoke();
    }

    public void ConnectPlayer()
    {
        FindObjectsByType<Character>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .ToList()
            .ForEach(x =>
            {
                if (x.CharacterType == Character.CharacterTypes.Player)
                {
                    x.SetInputManager(_inputManager);
                }
            });
    }

    private void OnScreenChanged(UIScreenType oldScreen, UIScreenType newScreen)
    {
        bool isActive = screensMask.HasFlag((UIScreenTypes)(int)newScreen);
        _joystick.gameObject.SetActive(isActive);
    }
}
