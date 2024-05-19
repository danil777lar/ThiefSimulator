using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootCase : MonoBehaviour
{
    private Camera caseCamera;
    private RenderTexture texture;
    
    public RenderTexture GetTexture(int width, int height)
    {
        texture ??= new RenderTexture(width, height, 24);
        caseCamera.targetTexture = texture;
        return texture;
    }

    private void Awake()
    {
        caseCamera = GetComponentInChildren<Camera>();
    }

    private void OnDestroy()
    {
        if (texture != null)
        {
            Destroy(texture);
        }
    }
}
