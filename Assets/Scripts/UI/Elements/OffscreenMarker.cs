using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class OffscreenMarker : MonoBehaviour
{
    [SerializeField] private RectTransform iconRoot;
    
    private Transform _target;
    private Func<bool> _isActive;

    public OffscreenMarker Init(Transform target, Func<bool> isActive)
    {
        _target = target;
        _isActive = isActive;
        
        return this;
    }
    
    private void Update()
    {
        if (_target == null) return;
        
        Vector3 screenPos = Camera.main.WorldToScreenPoint(_target.position);
        bool isOnScreen = screenPos.x > 0f && screenPos.x < Screen.width && screenPos.y > 0f && screenPos.y < Screen.height;
        iconRoot.gameObject.SetActive(!isOnScreen && _isActive.Invoke());

        Vector2 size = iconRoot.sizeDelta; 
        Vector3 iconRootPosition = screenPos;
        iconRootPosition.x = Mathf.Clamp(iconRootPosition.x, size.x * 0.5f, Screen.width - size.x * 0.5f);
        iconRootPosition.y = Mathf.Clamp(iconRootPosition.y, size.y * 0.5f, Screen.height - size.y * 0.5f);
        iconRoot.position = iconRootPosition;
    }
}
