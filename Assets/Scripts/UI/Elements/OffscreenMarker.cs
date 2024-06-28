using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

public class OffscreenMarker : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float distanceFromCenterPercent = 0.5f;

    [SerializeField] private bool useOnscreenDistance = true;
    [SerializeField, Range(0f, 1f)] private float distanceToTargetPercent = 0.25f;
    
    [Header("Links")]
    [SerializeField] private RectTransform iconRoot;
    [SerializeField] private RectTransform arrow;

    private Vector3 _centerPosition;
    
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
        
        return this;
    }

    private void FixedUpdate()
    {
        _centerPosition = _camera.WorldToScreenPoint(_center.position);
    }

    private void Update()
    {
        if (_target == null) return;

        Vector3 screenPositionRaw = _camera.WorldToScreenPoint(_target.position);
        if (screenPositionRaw.z < 0f)
        {
            screenPositionRaw *= -1f;
        }
        
        Vector3 direction = (screenPositionRaw - _centerPosition).normalized;
        float distance = Screen.width * 0.5f * distanceFromCenterPercent;
        Vector3 screenPositionFixed = _centerPosition + direction * distance;
        
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
            arrow.localRotation = arrowRotation;
        }
    }
}
