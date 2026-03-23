using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class OffscreenMarker : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float distanceFromCenterPercent = 0.5f;

    [SerializeField] private bool useOnscreenDistance = true;
    [SerializeField, Range(0f, 1f)] private float distanceToTargetPercent = 0.25f;
    
    [Header("Links")]
    [SerializeField] private RectTransform iconRoot;
    [SerializeField] private RectTransform back;
    [SerializeField] private RectTransform arrow;

    private Vector3 _targetCenterPosition;
    private Vector3 _currentCenterPosition;
    
    private Camera _camera;
    private CanvasGroup _canvasGroup;
    
    private Transform _target;
    private Transform _center;
    
    private Func<bool> _isActive;

    public OffscreenMarker Init(Transform target, Transform center, Func<bool> isActive)
    {
        _camera = Camera.main;
        
        _target = target;
        _center = center;
        _isActive = isActive;
        
        _canvasGroup = GetComponent<CanvasGroup>();

        _currentCenterPosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        
        return this;
    }

    private void FixedUpdate()
    {
        _targetCenterPosition = _camera.WorldToScreenPoint(_center.position);
    }

    private void Update()
    {
        _currentCenterPosition = Vector3.Lerp(_currentCenterPosition, _targetCenterPosition, Time.deltaTime * 2f);
        
        if (_target == null) return;

        Vector3 screenPositionRaw = _camera.WorldToScreenPoint(_target.position);
        if (screenPositionRaw.z < 0f)
        {
            screenPositionRaw *= -1f;
        }
        
        Vector3 direction = (screenPositionRaw - _currentCenterPosition).normalized;
        float distance = Screen.width * 0.5f * distanceFromCenterPercent;
        Vector3 screenPositionFixed = _currentCenterPosition + direction * distance;
        
        bool targetOnScreen = screenPositionRaw.x > 0f && screenPositionRaw.x < Screen.width && 
                              screenPositionRaw.y > 0f && screenPositionRaw.y < Screen.height;
        
        bool distanceOk = useOnscreenDistance && 
                          Vector2.Distance(screenPositionRaw, screenPositionFixed) > 
                          (Screen.width * 0.5f * distanceToTargetPercent);
        
        bool isActive = (!targetOnScreen || distanceOk) && _isActive.Invoke();
        
        _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, isActive ? 1f : 0f, Time.deltaTime * 10f);
        iconRoot.position = screenPositionFixed;

        if (arrow != null)
        {
            float arrowAngle = Vector2.SignedAngle(Vector2.right, screenPositionRaw - iconRoot.position);
            Quaternion arrowRotation = Quaternion.Euler(Vector3.forward * arrowAngle);
            arrow.localRotation = Quaternion.Lerp(arrow.localRotation, arrowRotation, Time.deltaTime * 10f);
        }
        
        if (back != null)
        {
            back.rotation = Quaternion.Euler(Vector3.zero);
        }
    }
}
