using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using MoreMountains.Feedbacks;
using ProjectConstants;
using UnityEngine;
using UnityEngine.Events;

public class SimplePlayerInteractionPoint : MonoBehaviour, IMiniGameLauncher
{
    [SerializeField] private LayerMask mask;
    [SerializeField] private float duration;
    [Space]
    [SerializeField] private bool playMiniGame;
    [SerializeField] private MiniGameType miniGamePopupType;
    [Space]
    [SerializeField] private UnityEvent eventOnComplete;
    [SerializeField] private UnityEvent eventOnFail;
    
    [InjectService] private MiniGameLauncherService _miniGameLauncherService;

    private float _timer;
    private Collider _other;
    
    private List<Func<float>> _multipliers = new List<Func<float>>();
    
    public MiniGameType MiniGameType => miniGamePopupType;

    public void AddMultiplier(Func<float> multiplier)
    {
        if (!_multipliers.Contains(multiplier))
        {
            _multipliers.Add(multiplier);
        }
    }

    public void RemoveMultiplier(Func<float> multiplier)
    {
        if (_multipliers.Contains(multiplier))
        {
            _multipliers.Remove(multiplier);
        }
    }
    
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
            _miniGameLauncherService.LaunchMiniGame(
                miniGamePopupType,
                GetMultiplier(), 
                () => eventOnComplete.Invoke(), 
                () => eventOnFail.Invoke());
        }
        else
        {
            eventOnComplete?.Invoke();   
        }
    }

    private float GetMultiplier()
    {
        float result = 1f;
        foreach (Func<float> multiplier in _multipliers)
        {
            result *= multiplier.Invoke();
        }
        return result;
    }
}
