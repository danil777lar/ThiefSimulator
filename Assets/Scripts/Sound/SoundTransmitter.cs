using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Tools;
using UnityEngine;

public class SoundTransmitter : MonoBehaviour
{
    [SerializeField] private float _soundSpeed;

    private float _initialAmplitude;
    private Color _baseColor;
    private MeshRenderer _renderer;

    public float CurrentAmplitude { get; private set; }

    public void Init(float amplitude)
    {
        _renderer = GetComponentInChildren<MeshRenderer>();
        _baseColor = _renderer.material.GetColor("_BaseColor");

        _initialAmplitude = amplitude;
        CurrentAmplitude = amplitude;

        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        float delta = _soundSpeed * Time.deltaTime;
        CurrentAmplitude -= delta;
        transform.localScale += Vector3.one * delta;
        if (CurrentAmplitude <= 0f)
        {
            OnComplete();
        }
        
        if (_renderer)
        {
            float alpha = CurrentAmplitude / _initialAmplitude;
            _renderer.material.SetColor("_BaseColor", _baseColor.SetAlpha(alpha * _baseColor.a));
        }   
    }

    private void OnComplete()
    {
        Destroy(gameObject);
    }
}
