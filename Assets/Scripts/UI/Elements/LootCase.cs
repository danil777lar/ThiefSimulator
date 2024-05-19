using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Serialization;

public class LootCase : MonoBehaviour
{
    [SerializeField] private MMF_Player showFeedback;
    [SerializeField] private MMF_Player openFeedback;
    
    private Camera caseCamera;
    private RenderTexture texture;

    public event Action EventShown;
    public event Action EventOpened;
    
    public RenderTexture Show(int width, int height)
    {
        texture ??= new RenderTexture(width, height, 24);
        caseCamera.targetTexture = texture;
        
        showFeedback?.PlayFeedbacks();
        
        return texture;
    }

    public void Open()
    {
        openFeedback?.PlayFeedbacks();
    }

    private void Awake()
    {
        caseCamera = GetComponentInChildren<Camera>();
        
        showFeedback.Events.OnComplete.AddListener(() => EventShown?.Invoke());
        openFeedback.Events.OnComplete.AddListener(() => EventOpened?.Invoke());
    }

    private void OnDestroy()
    {
        if (texture != null)
        {
            Destroy(texture);
        }
    }
}
