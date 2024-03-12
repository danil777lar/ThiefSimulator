using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LockMiniGamePopup : MiniGamePopup
{
    [SerializeField] private float cylinderRotateSpeed;
    [SerializeField] private float lockerRotateSpeed;
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

    private bool _rotateCylinder;
    private float _cylinderValue = 0f;
    private float _unlockerValue = 0.5f;
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

            CreateTexture();
        }
    }

    private void Update()
    {
        _cylinderValue = _rotateCylinder ? 
            Mathf.Clamp01(_cylinderValue + Time.deltaTime * cylinderRotateSpeed) : 
            Mathf.Clamp01(_cylinderValue - Time.deltaTime * cylinderRotateSpeed);
        
        cylinder.transform.localRotation = Quaternion.Euler(
            Vector3.Lerp(Vector3.zero, Vector3.up * -90f, _cylinderValue));
        
        locker.transform.localRotation = Quaternion.Euler( 
            Vector3.Lerp(Vector3.up * 90f, Vector3.up * -90f, _unlockerValue) - 
            cylinder.transform.localRotation.eulerAngles.Y());
    }

    private void OnDestroy()
    {
        DestroyTexture();
    }

    private void RotateLocker(PointerEventData data)
    {
        _unlockerValue = Mathf.Clamp01(_unlockerValue + (data.delta.x / Screen.width) * lockerRotateSpeed);
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
        _args.OnComplete?.Invoke();
        Close();
    }
    
    private void Fail()
    {
        _args.OnFail?.Invoke();
        Close();
    }
    
    private void Exit()
    {
        Close();
    }

    public class Args : MiniGamePopup.MiniGameArgs
    {
        public Args(Action onComplete, Action onFail) : base(UIPopupType.LockMiniGame, onComplete, onFail)
        {
        }
    }    
}
