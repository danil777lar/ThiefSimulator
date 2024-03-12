using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services.UI;
using MoreMountains.Tools;
using ProjectConstants;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LockMiniGamePopup : MiniGamePopup
{
    [SerializeField] private float cylinderRotateSpeed;
    [SerializeField] private float lockerRotateSpeed;
    [SerializeField] private float unlockerDamageSpeed;
    [SerializeField] private float unlockRange;
    [Space] 
    [SerializeField] private Transform cylinder;
    [SerializeField] private Transform locker;
    [Space] 
    [SerializeField] private Camera miniGameCamera;
    [SerializeField] private RawImage renderImage;
    [Space] 
    [SerializeField] private RectTransformEvents rotateCylinderRect;
    [SerializeField] private RectTransformEvents rotateLockerRect;
    [Space] 
    [SerializeField] private Button exitButton;

    private bool _controlActive;
    private bool _rotateCylinder;
    private float _unlockerHealth = 1f;
    private float _cylinderValue = 0f;
    private float _unlockerValue = 0.5f;
    private float _unlockPoint;
    private MiniGameArgs _args;
    private RenderTexture _texture;

    protected override void OnOpened(PopupOpenProperties args)
    {
        base.OnOpened(args);

        if (args is MiniGameArgs miniGameArgs)
        {
            _args = miniGameArgs;
            
            exitButton.onClick.AddListener(Exit);
            rotateLockerRect.EventPointerDrag += RotateLocker;
            rotateCylinderRect.EventPointerDown += (x) => _rotateCylinder = true;
            rotateCylinderRect.EventPointerUp += (x) => _rotateCylinder = false;

            _controlActive = true;
            _unlockerHealth = 1f;
            _unlockPoint = Random.Range(0f, 1f);

            CreateTexture();
        }
    }

    private void Update()
    {
        _cylinderValue = _rotateCylinder && _controlActive ? 
            Mathf.Clamp01(_cylinderValue + Time.deltaTime * cylinderRotateSpeed) : 
            Mathf.Clamp01(_cylinderValue - Time.deltaTime * cylinderRotateSpeed);
        _cylinderValue = Mathf.Clamp(_cylinderValue, 0f, GetDistanceToUnlock());

        cylinder.transform.localRotation = Quaternion.Euler(
            Vector3.Lerp(Vector3.zero, Vector3.up * -90f, _cylinderValue));
        
        locker.transform.localRotation = Quaternion.Euler( 
            Vector3.Lerp(Vector3.up * 90f, Vector3.up * -90f, _unlockerValue) - 
            cylinder.transform.localRotation.eulerAngles.Y());

        if (_controlActive)
        {
            if (_cylinderValue >= 1f)
            {
                Complete();
            }
            else if (_cylinderValue >= GetDistanceToUnlock())
            {
                _unlockerHealth -= unlockerDamageSpeed * Time.deltaTime;
                if (_unlockerHealth <= 0f)
                {
                    Fail();
                }
            }
        }
    }

    private void OnDestroy()
    {
        DestroyTexture();
    }

    private void RotateLocker(PointerEventData data)
    {
        if (_controlActive)
        {
            _unlockerValue = Mathf.Clamp01(_unlockerValue + (data.delta.x / Screen.width) * lockerRotateSpeed);
        }
    }
    
    private void CreateTexture()
    {
        _texture = new RenderTexture(Screen.width, Screen.height, 16);
        miniGameCamera.targetTexture = _texture;
        renderImage.texture = _texture;
    }

    private void DestroyTexture()
    {
        if (_texture != null)
        {
            miniGameCamera.targetTexture = null;
            Destroy(_texture);
        }
    }

    private void Complete()
    {
        _controlActive = false;
        _args.OnComplete?.Invoke();
        Close();
    }
    
    private void Fail()
    {
        _controlActive = false;
        _args.OnFail?.Invoke();
        Close();
    }
    
    private void Exit()
    {
        Close();
    }
    
    private float GetDistanceToUnlock()
    { 
        return 1f - Mathf.Clamp01(Mathf.Abs(_unlockerValue - _unlockPoint) - unlockRange); 
    }

    public class Args : MiniGamePopup.MiniGameArgs
    {
        public Args(Action onComplete, Action onFail) : base(UIPopupType.LockMiniGame, onComplete, onFail)
        {
        }
    }    
}
