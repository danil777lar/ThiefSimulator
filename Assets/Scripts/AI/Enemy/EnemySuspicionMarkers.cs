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
    [SerializeField] private RectTransform suspicionGlow;
    [SerializeField] private Image suspicionProgress;
    [Space]
    [SerializeField] private RectTransform aggressionRoot;
    [SerializeField] private RectTransform aggressionGlow;
    [SerializeField] private Image aggressionProgress;

    private Camera _camera;
    private EnemyAttention _enemyAttention;
    private CharacterController _characterController;
    
    private void Start()
    {
        _camera = Camera.main;
        _enemyAttention = GetComponentInParent<EnemyAttention>();
        _characterController = GetComponentInParent<CharacterController>();
    }
    
    private void FixedUpdate()
    {
        TryUpdateSuspicion();
        TryUpdateAggression();
    }

    private void TryUpdateSuspicion()
    {
        bool suspicionActive = _enemyAttention.AttentionLevel > 0f;
        suspicionActive &= _enemyAttention.AttentionLevel <= _enemyAttention.MaxSuspicion;
        suspicionRoot.gameObject.SetActive(suspicionActive);
        if (suspicionActive)
        {
            suspicionProgress.fillAmount = _enemyAttention.AttentionLevel / _enemyAttention.MaxSuspicion;
            UpdatePosition();
        }
        
        suspicionGlow.gameObject.SetActive(_enemyAttention.CurrentState == 
                                           EnemyAttention.AttentionState.Suspicious);
    }
    
    private void TryUpdateAggression()
    {
        bool aggressionActive = _enemyAttention.AttentionLevel > _enemyAttention.MaxSuspicion;
        aggressionRoot.gameObject.SetActive(aggressionActive);
        if (aggressionActive)
        {
            aggressionProgress.fillAmount = (_enemyAttention.AttentionLevel - _enemyAttention.MaxSuspicion) /
                                            _enemyAttention.MaxAggression;
            UpdatePosition();
        }
        
        aggressionGlow.gameObject.SetActive(_enemyAttention.CurrentState == 
                                           EnemyAttention.AttentionState.Aggressive);
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
//