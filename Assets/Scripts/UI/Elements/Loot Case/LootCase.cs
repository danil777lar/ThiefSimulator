using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LootCase : MonoBehaviour
{
    private Camera _caseCamera;
    private RenderTexture _texture;
    private LootCaseAnimation _lootCaseAnimation;

    public event Action EventShown;
    public event Action EventOpened;
    
    public RenderTexture Show(int width, int height)
    {
        _texture ??= new RenderTexture(width, height, 24);
        _caseCamera.targetTexture = _texture;

        _lootCaseAnimation.PlayShowAnimation(() => EventShown?.Invoke());
        
        return _texture;
    }

    public void Open()
    {
        _lootCaseAnimation.PlayOpenAnimation(() => EventOpened?.Invoke());
    }

    private void Awake()
    {
        _lootCaseAnimation = GetComponent<LootCaseAnimation>();
        _caseCamera = GetComponentInChildren<Camera>();
    }

    private void OnDestroy()
    {
        if (_texture != null)
        {
            Destroy(_texture);
        }
    }
}
