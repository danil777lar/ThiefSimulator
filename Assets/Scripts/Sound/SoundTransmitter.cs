using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundTransmitter : MonoBehaviour
{
    [SerializeField] private bool updateMaterial;
    [SerializeField] private float soundSpeed = 5f;
    [SerializeField] private Gradient colorOverLifetime;

    private float _initialAmplitude;
    private Color _baseColor;
    private Mesh _mesh;

    public float CurrentAmplitude { get; private set; }

    public void Init(float amplitude)
    {
        _initialAmplitude = amplitude;
        CurrentAmplitude = amplitude;

        transform.localScale = Vector3.zero;
        GrabMesh();
    }

    private void Update()
    {
        float delta = soundSpeed * Time.deltaTime;
        CurrentAmplitude -= delta;
        transform.localScale += Vector3.one * delta;
        if (CurrentAmplitude <= 0f)
        {
            OnComplete();
        }
        
        UpdateMaterial();
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
        if (_mesh)
        {
            float lifetime = 1f - (CurrentAmplitude / _initialAmplitude);
            List<Color> colors = new List<Color>();
            for (int i = 0; i < _mesh.vertexCount; i++)
            {
                colors.Add(colorOverLifetime.Evaluate(lifetime));
            }
            _mesh.colors = colors.ToArray();
        }   
    }

    private void GrabMesh()
    {
        if (updateMaterial)
        {
            MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
            if (meshFilter)
            {
                _mesh = new Mesh();
                _mesh.vertices = meshFilter.mesh.vertices;
                _mesh.triangles = meshFilter.mesh.triangles;
                _mesh.uv = meshFilter.mesh.uv;

                meshFilter.mesh = _mesh;
            }
        }
    }

    private void OnComplete()
    {
        Destroy(gameObject);
    }
}
