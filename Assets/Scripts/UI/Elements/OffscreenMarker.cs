using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class OffscreenMarker : MonoBehaviour
{
    [SerializeField] private float distance;
    [SerializeField] private RectTransform iconRoot;
    [SerializeField] private RectTransform arrow;

    private CanvasGroup _canvasGroup;
    private Transform _target;
    private Func<bool> _isActive;

    public OffscreenMarker Init(Transform target, Func<bool> isActive)
    {
        _target = target;
        _isActive = isActive;
        
        _canvasGroup = GetComponent<CanvasGroup>();
        
        return this;
    }
    
    private void Update()
    {
        if (_target == null) return;

        Vector3 centerPosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(_target.position);
        
        bool isOnScreen = screenPos.x > 0f && screenPos.x < Screen.width && screenPos.y > 0f && screenPos.y < Screen.height;
        bool isActive = !isOnScreen && _isActive.Invoke();
        _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, isActive ? 1f : 0f, Time.deltaTime * 10f);
        
        Vector3 direction = (screenPos - centerPosition).normalized;
        iconRoot.position = centerPosition + direction * distance;

        if (arrow != null)
        {
            float arrowAngle = Vector2.SignedAngle(Vector2.right, screenPos - iconRoot.position);
            Quaternion arrowRotation = Quaternion.Euler(Vector3.forward * arrowAngle);
            arrow.localRotation = arrowRotation;
        }
    }
}
