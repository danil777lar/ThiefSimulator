using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterVisionTarget : MonoBehaviour
{
    private static List<CharacterVisionTarget> _targets = new List<CharacterVisionTarget>();
    public static IReadOnlyCollection<CharacterVisionTarget> Targets => _targets.AsReadOnly();

    [SerializeField] private float delayOnDamageDecline = 2f;
    [SerializeField] private float defaultVisibility = 1f;
    
    private List<Func<float>> _visibilityMultipliers = new List<Func<float>>();
    
    public Character Character { get; private set; }

    public float GetVisibility()
    {
        float visibility = defaultVisibility;

        foreach (Func<float> multiplier in _visibilityMultipliers)
        {
            visibility *= multiplier.Invoke();
        }
        
        return visibility;
    }
    
    public void AddVisibilityMultiplier(Func<float> multiplier)
    {
        if (!_visibilityMultipliers.Contains(multiplier))
        {
            _visibilityMultipliers.Add(multiplier);
        }
    }

    public void RemoveVisibilityMultiplier(Func<float> multiplier)
    {
        if (_visibilityMultipliers.Contains(multiplier))
        {
            _visibilityMultipliers.Remove(multiplier);
        }
    }
    
    private void Start()
    {
        Character = GetComponentInParent<Character>();
        _targets.Add(this);
    }

    private void OnDestroy()
    {
        _targets.Remove(this);
    }
}
