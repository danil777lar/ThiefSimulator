using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.Events;

public class MiniGameLauncher : MonoBehaviour, IMiniGameLauncher
{
    [SerializeField] private bool playMiniGame;
    [SerializeField] private MiniGameType miniGamePopupType;
    
    [InjectService] private MiniGameLauncherService _miniGameLauncherService;
    
    public UnityEvent eventOnComplete;
    public UnityEvent eventOnFail;
    private List<Func<float>> _multipliers = new List<Func<float>>();
    
    public MiniGameType MiniGameType => miniGamePopupType;

    public void LaunchMiniGame()
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
        DIContainer.InjectTo(this);
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
