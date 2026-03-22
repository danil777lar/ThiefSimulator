using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using Larje.Core.Tools;
using ProjectConstants;
using UnityEngine;
using UnityEngine.InputSystem;

[BindService(typeof(PlayerInputService), typeof(InputService))]
public class PlayerInputService : InputService
{
    [SerializeField] private UIScreenTypes screensMask;
    [SerializeField] private CorePlayerJoystick joystick;
    
    [InjectService] private UIService _uiService;

    private bool _isTouching;
    private Vector3 _startTouchPosition;
    private Vector2 _currentTouchPosition;

    public event Action EventPointerDown;

    public override Vector2 PlayerMovement => joystick.GetNormalizedValue();
    public override InputAction UIBack => GetActions<InputSystem_Actions.UIActions>().Back;
    public override InputAction UIDebug => null;
    public override InputAction PlayerRun => GetActions<InputSystem_Actions.PlayerActions>().Run;
    public override InputAction PlayerPointer => GetActions<InputSystem_Actions.PlayerActions>().Pointer;

    public override Dictionary<InputActionMapType, Type> ActionMapTypes => null;
    public override Dictionary<Type, bool> DefaultStates => null;

    public override void Init()
    {
        base.Init();
        joystick.EventPointerDown += PointerDown;
    }

    public void PointerDown(Vector2 position)
    {
        EventPointerDown?.Invoke();
    }
}
