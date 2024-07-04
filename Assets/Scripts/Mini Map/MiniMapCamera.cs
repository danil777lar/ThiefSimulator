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

    [Header("Dynamic Layers")] 
    [SerializeField, Range(0.01f, 1f)] private float alphaModifier = 1f; 
    [SerializeField] private LayerMask dynamicLayers;
    
    [Header("Static Layers")]
    [SerializeField] private List<StaticLayer> staticLayers;
    
    [Header("Shaders")] 
    [SerializeField] private Shader rewriteShader;
    [SerializeField] private Shader copyShader;
    [SerializeField] private Shader copyCameraShader;

    private bool _initialized;
    private Camera _camera;
    private RenderTexture _cameraTexture;

    private Material _rewriteMaterial;
    private Material _copyMaterial;
    private Material _copyCameraMaterial;
    
    public RenderTexture StaticTexture { get; private set; }
    public RenderTexture DynamicTexture { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    
    private IEnumerator Start()
    {
        yield return null;

        _rewriteMaterial = new Material(rewriteShader);
        _copyMaterial = new Material(copyShader);
        _copyCameraMaterial = new Material(copyCameraShader);
        
        _camera = GetComponent<Camera>();

        DynamicTexture = CreateTexture();
        _cameraTexture = CreateTexture();
        StaticTexture = CreateTexture();
        
        _camera.targetTexture = _cameraTexture;
        
        yield return StartCoroutine(CreateStaticTexture());
        
        _camera.cullingMask = dynamicLayers;

        yield return null;

        _initialized = true;
    }

    private void Update()
    {
        if (_initialized)
        {
            ClearTexture(DynamicTexture);
            _copyCameraMaterial.SetFloat("_AlphaModifier", alphaModifier);
            Graphics.Blit(_cameraTexture, DynamicTexture, _copyCameraMaterial, 0);
        }
    }


    private IEnumerator CreateStaticTexture()
    {
        RenderTexture outTexture = CreateTexture();
        ClearTexture(StaticTexture);
        
        foreach (StaticLayer layer in staticLayers)
        {
            ClearTexture(outTexture);
            ClearTexture(_cameraTexture);

            _camera.targetTexture = _cameraTexture;
            _camera.cullingMask = layer.Layer;
            _camera.Render();
            
            _copyMaterial.SetTexture("_Prev", StaticTexture);
            _copyMaterial.SetColor("_Color", layer.Color);
            
            Graphics.Blit(_cameraTexture, outTexture, _copyMaterial, 0);
            Graphics.Blit(outTexture, StaticTexture, _rewriteMaterial, 0);
        } 
        
        
        Graphics.Blit(StaticTexture, DynamicTexture, _rewriteMaterial, 0);
        
        Destroy(outTexture);

        yield return null;
    }

    private RenderTexture CreateTexture()
    {
        RenderTexture texture = new RenderTexture(mapPixelSize.x, mapPixelSize.y, 
            GraphicsFormat.R8G8B8A8_UNorm, GraphicsFormat.D16_UNorm);
        texture.filterMode = FilterMode.Bilinear;
        texture.Create();

        return texture;
    }

    private void ClearTexture(RenderTexture texture)
    {
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = texture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = rt;
    }

    [Serializable]
    private class StaticLayer
    {
        [field: SerializeField] public LayerMask Layer { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }
    } 
}
