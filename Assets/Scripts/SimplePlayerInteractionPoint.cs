using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using MoreMountains.Feedbacks;
using ProjectConstants;
using UnityEngine;
using UnityEngine.Events;

public class SimplePlayerInteractionPoint : MonoBehaviour
{
    [SerializeField] private LayerMask mask;
    [SerializeField] private float duration;
    [Space]
    [SerializeField] private bool playMiniGame;
    [SerializeField] private UIPopupType miniGamePopupType;
    [Space]
    [SerializeField] private UnityEvent eventOnComplete;
    [SerializeField] private UnityEvent eventOnFail;

    [InjectService] private UIService _uiService;

    private float _timer;
    private Collider _other;

    private void Start()
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
    }
    
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
        if (playMiniGame)
        {
            _uiService.GetProcessor<UIPopupProcessor>().OpenPopup(
                new MiniGamePopup.MiniGameArgs(miniGamePopupType, 
                    () => eventOnComplete.Invoke(), 
                    () => eventOnFail.Invoke()));
        }
        else
        {
            eventOnComplete?.Invoke();   
        }
    }
}
