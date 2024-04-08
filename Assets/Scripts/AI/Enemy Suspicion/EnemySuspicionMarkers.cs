using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySuspicionMarkers : MonoBehaviour
{
    [SerializeField] private float heightOffset;
    [SerializeField] private RectTransform markersRoot;
    [Space]
    [SerializeField] private RectTransform suspicionRoot;
    [SerializeField] private Image suspicionProgress;
    [Space]
    [SerializeField] private RectTransform aggressionRoot;
    [SerializeField] private Image aggressionProgress;

    private Camera _camera;
    private CharacterAttention _characterAttention;
    private CharacterController _characterController;
    
    private void Start()
    {
        _camera = Camera.main;
        _characterAttention = GetComponentInParent<CharacterAttention>();
        _characterController = GetComponentInParent<CharacterController>();
    }
    
    private void FixedUpdate()
    {
        TryUpdateSuspicion();
        TryUpdateAggression();
    }

    private void TryUpdateSuspicion()
    {
        bool suspicionActive = _characterAttention.Suspicion > 0f;
        suspicionActive &= _characterAttention.Aggression <= 0f;
        suspicionRoot.gameObject.SetActive(suspicionActive);
        if (suspicionActive)
        {
            suspicionProgress.fillAmount = _characterAttention.Suspicion / _characterAttention.MaxSuspicion;
            UpdatePosition();
        }
    }
    
    private void TryUpdateAggression()
    {
        bool aggressionActive = _characterAttention.Aggression > 0f;
        aggressionRoot.gameObject.SetActive(aggressionActive);
        if (aggressionActive)
        {
            aggressionProgress.fillAmount = _characterAttention.Aggression / 1f;
            UpdatePosition();
        }
    }
    
    private void UpdatePosition()
    {
        Vector2 sideOffset = markersRoot.sizeDelta * 0.5f;
        Vector3 position = _characterController.transform.position + 
                           Vector3.up * (heightOffset + _characterController.height);
        position = _camera.WorldToScreenPoint(position);

        position.x = Mathf.Clamp(position.x, 0f + sideOffset.x, Screen.width - sideOffset.x);
        position.y = Mathf.Clamp(position.y, 0f, Screen.height - sideOffset.y * 2f);
        
        markersRoot.transform.position = position;
    }
}
