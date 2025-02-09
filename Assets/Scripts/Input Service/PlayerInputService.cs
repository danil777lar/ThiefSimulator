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

[BindService(typeof(PlayerInputService), typeof(InputService))]
public class PlayerInputService : InputService
{
    [SerializeField] private UIScreenTypes screensMask; 
    
    [InjectService] private UIService _uiService;

    private bool _isTouching;
    private Vector3 _startTouchPosition;
    private Vector2 _currentTouchPosition;

    public event Action EventPointerDown;

    public override InputAction UIBack => GetActions<InputSystem_Actions.UIActions>().Back;
    public override InputAction PlayerRun => GetActions<InputSystem_Actions.PlayerActions>().Run;

    public override void Init()
    {
        base.Init();
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
                    
                }
            });
    }

    protected override void Update()
    {
        base.Update();
        MMDebug.DebugOnScreen($"press: {_isTouching}\n cur: {_currentTouchPosition.ToString()}\n start: {_startTouchPosition.ToString()}");

        InputSystem_Actions.PlayerActions playerActions = GetActions<InputSystem_Actions.PlayerActions>();
        Vector3 pointerValue = playerActions.Pointer.ReadValue<Vector3>();
        _currentTouchPosition = pointerValue.XY();
        if (pointerValue.z > 0)
        {
            if (!_isTouching)
            {
                _startTouchPosition = _currentTouchPosition;
                _isTouching = true;
            }
        }
        else
        {
            _isTouching = false;
        }
    }
    
    
}
