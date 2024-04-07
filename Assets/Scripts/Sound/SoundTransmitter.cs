using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Tools;
using UnityEngine;

public class SoundTransmitter : MonoBehaviour
{
    private float _speed;
    private float _maxDistance;
    private Color _baseColor;
    private MeshRenderer _renderer;

    public float Value { get; private set; }

    public void Init(float speed, float maxDistance)
    {
        _speed = speed;
        _maxDistance = maxDistance;
        
        _renderer = GetComponentInChildren<MeshRenderer>();
        _baseColor = _renderer.material.GetColor("_BaseColor");
        
        float duration = _maxDistance / _speed; 
        transform.localScale = Vector3.zero;
        transform.DOScale(_maxDistance, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(OnComplete);
    }

    private void Update()
    {
        Value = transform.localScale.x / _maxDistance;
        if (_renderer)
        {
            _renderer.material.SetColor("_BaseColor", _baseColor.SetAlpha((1f - Value) * _baseColor.a));
        }   
    }

    private void OnComplete()
    {
        Destroy(gameObject);
    }
}
