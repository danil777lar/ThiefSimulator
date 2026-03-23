using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LootCase : MonoBehaviour
{
    [SerializeField, Min(0f)] private float showEventDelayMultiplier = 1f;
    // [SerializeField] private MMF_Player showFeedback;
    [Space]
    [SerializeField, Min(0f)] private float openEventDelayMultiplier = 1f;
    // [SerializeField] private MMF_Player openFeedback;
    
    private Camera caseCamera;
    private RenderTexture texture;

    public event Action EventShown;
    public event Action EventOpened;
    
    public RenderTexture Show(int width, int height)
    {
        texture ??= new RenderTexture(width, height, 24);
        caseCamera.targetTexture = texture;
        
        // showFeedback?.PlayFeedbacks();
        StartCoroutine(ShowEventDelay());
        
        return texture;
    }

    public void Open()
    {
        // openFeedback?.PlayFeedbacks();
        StartCoroutine(OpenEventDelay());
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

    private IEnumerator ShowEventDelay()
    {
        // yield return new WaitForSeconds(showFeedback.TotalDuration * showEventDelayMultiplier);
        yield return new WaitForSeconds(1f);
        EventShown?.Invoke();
    }
    
    private IEnumerator OpenEventDelay()
    {
        // yield return new WaitForSeconds(openFeedback.TotalDuration * openEventDelayMultiplier);
        yield return new WaitForSeconds(1f);
        EventOpened?.Invoke();
    }
}
