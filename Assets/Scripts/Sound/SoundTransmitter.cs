using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundTransmitter : MonoBehaviour
{
    [SerializeField] private bool updateMaterial;
    [SerializeField] private float soundSpeed = 5f;
    [SerializeField] private Gradient colorOverLifetime;
    [SerializeField] private AnimationCurve heightCurve;

    private float _defaultHeight;
    private float _initialAmplitude;
    private Color _baseColor;
    private Mesh _mesh;
    private MeshRenderer _meshRenderer;

    public float CurrentAmplitude { get; private set; }

    public void Init(float amplitude)
    {
        _initialAmplitude = amplitude;
        CurrentAmplitude = amplitude;

        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _defaultHeight = transform.localScale.y;
        transform.localScale = new Vector3(0f, _defaultHeight, 0f);
    }

    private void Update()
    {
        float delta = soundSpeed * Time.deltaTime;
        CurrentAmplitude -= delta;
        transform.localScale += new Vector3(1f, 0f, 1f) * delta;
        if (CurrentAmplitude <= 0f)
        {
            OnComplete();
        }
        
        UpdateMaterial();
        UpdateHeight();
    }

    private void OnDestroy()
    {
        if (_mesh != null)
        {
            Destroy(_mesh);
        }
    }

    private void UpdateMaterial()
    {
        float lifetime = 1f - (CurrentAmplitude / _initialAmplitude);
        Color color = colorOverLifetime.Evaluate(lifetime);
        _meshRenderer.material.SetColor("_BaseColor", color);
    }

    private void UpdateHeight()
    {
        float lifetime = 1f - (CurrentAmplitude / _initialAmplitude);
        Vector3 scale = transform.localScale;
        scale.y = _defaultHeight * heightCurve.Evaluate(lifetime);
        transform.localScale = scale;
    }

    private void OnComplete()
    {
        Destroy(gameObject);
    }
}
