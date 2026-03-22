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

using PlayerActions = InputSystem_Actions.PlayerActions;
using UIActions = InputSystem_Actions.UIActions;

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
    public override InputAction UIBack => GetActions<UIActions>().Back;
    public override InputAction UIDebug => null;
    public override InputAction PlayerRun => GetActions<PlayerActions>().Run;
    public override InputAction PlayerPointer => GetActions<PlayerActions>().Pointer;

    public override Dictionary<InputActionMapType, Type> ActionMapTypes => new Dictionary<InputActionMapType, Type>
    {
        { InputActionMapType.Player, typeof(PlayerActions) },
        { InputActionMapType.UI, typeof(UIActions) },
    };
    
    public override Dictionary<Type, bool> DefaultStates => new Dictionary<Type, bool>
    {
        { typeof(PlayerActions), false },
        { typeof(UIActions), true },
    };

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
