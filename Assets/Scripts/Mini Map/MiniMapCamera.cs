using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[RequireComponent(typeof(Camera))]
public class MiniMapCamera : MonoBehaviour
{
    public static MiniMapCamera Instance { get; private set; }
    
    [SerializeField] private Vector2Int mapPixelSize;
    [SerializeField] private LayerMask dynamicLayers;
    [SerializeField] private List<StaticLayer> staticLayers;
    [Header("Shaders")] 
    [SerializeField] private Shader copyShader;
    [SerializeField] private Shader copyCameraShader;
    
    private Camera _camera;
    private RenderTexture _cameraTexture;
    private Texture2D _staticTexture;
    
    private Material _copyMaterial;
    private Material _copyCameraMaterial;
    
    public RenderTexture OutTexture { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    
    private IEnumerator Start()
    {
        yield return null;
        Initialize();
    }

    private void Update()
    {
        if (OutTexture)
        {
            _copyCameraMaterial.SetTexture("_Prev", _staticTexture);
            Graphics.Blit(_cameraTexture, OutTexture, _copyCameraMaterial);
        }
    }

    private void Initialize()
    {
        _copyMaterial = new Material(copyShader);
        _copyCameraMaterial = new Material(copyCameraShader);
        
        _camera = GetComponent<Camera>();
        
        OutTexture = new RenderTexture(mapPixelSize.x, mapPixelSize.y, 24);
        _cameraTexture = new RenderTexture(mapPixelSize.x, mapPixelSize.y, 24);
        _staticTexture = new Texture2D(mapPixelSize.x, mapPixelSize.y);
        
        CreateStaticTexture();

        _camera.cullingMask = dynamicLayers;
        _camera.targetTexture = _cameraTexture;
    }

    private void CreateStaticTexture()
    {
        RenderTexture fullResult = new RenderTexture(mapPixelSize.x, mapPixelSize.y, 24);
        
        foreach (StaticLayer layer in staticLayers)
        {
            _camera.cullingMask = layer.Layer;

            RenderTexture texture = new RenderTexture(mapPixelSize.x, mapPixelSize.y, 24);
            _camera.targetTexture = texture;
            _camera.Render();
            RenderTexture.active = texture;

            Texture2D layerResult = new Texture2D(mapPixelSize.x, mapPixelSize.y);
            layerResult.ReadPixels(new Rect(0, 0, mapPixelSize.x, mapPixelSize.y), 0, 0);
            layerResult.Apply();
            
            _copyMaterial.SetTexture("_Prev", fullResult);
            _copyMaterial.SetColor("_Color", layer.Color);
            
            Graphics.Blit(layerResult, texture, _copyMaterial);
            Graphics.Blit(texture, fullResult);
        }
        
        RenderTexture.active = fullResult;
        _staticTexture.ReadPixels(new Rect(0, 0, mapPixelSize.x, mapPixelSize.y), 0, 0);
        _staticTexture.Apply();
    }

    [Serializable]
    private class StaticLayer
    {
        [field: SerializeField] public LayerMask Layer { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }
    } 
}
