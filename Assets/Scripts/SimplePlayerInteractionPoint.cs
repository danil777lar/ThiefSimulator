using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class SimplePlayerInteractionPoint : MonoBehaviour
{
    [SerializeField] private LayerMask mask;
    [SerializeField] private float duration;
    [Space] 
    [SerializeField] private MMF_Player feedbackOnComplete;
    [SerializeField] private MMF_Player feedbackOnFail;

    private float _timer;
    private Collider _other;

    private void Update()
    {
        if (_other != null)
        {
            _timer += Time.deltaTime;
            if (_timer >= duration)
            {
                Interact();
                _other = null;
            }
        }
        else
        {
            _timer = 0f;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (_other == null)
        {
            if (mask.HasLayer(other.gameObject.layer))
            {
                _other = other;
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (_other != null && _other == other)
        {
            _other = null;
        }
    }

    private void Interact()
    {
        feedbackOnComplete?.PlayFeedbacks();
    }
}
