using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using ProjectConstants;
using UnityEngine;
using UnityEngine.InputSystem;

[BindService(typeof(PlayerInputService), typeof(IInputService))]
public class PlayerInputService : Service, IInputService
{
    [SerializeField] private UIScreenTypes screensMask; 
    
    [InjectService] private UIService _uiService;

    private bool _isActive = true;
    private InputManager _inputManager;
    private MMTouchJoystick _joystick;
    private CanvasGroup _group;
    
    public InputSystem_Actions.PlayerActions PlayerActions { get; }

    public event Action EventPointerDown; 
    
    public override void Init()
    {
        
        
        _inputManager = GetComponent<InputManager>();
        _joystick = GetComponentInChildren<MMTouchJoystick>(true);
        _uiService.GetProcessor<UIScreenProcessor>().EventScreenOpened += OnScreenChanged;
        _group = _joystick.GetComponent<CanvasGroup>();
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

    private void OnScreenChanged(UIScreenType newScreen)
    {
        _isActive = screensMask.HasFlag((UIScreenTypes)(int)newScreen);
        if (_isActive)
        {
            _joystick.gameObject.SetActive(true);
        }
        else
        {
            _group.alpha = 0;
        }
    }
    
    private void OnJoystickPointerUp()
    {
        if (!_isActive)
        {
            _joystick.gameObject.SetActive(false);
        }
    }
}
