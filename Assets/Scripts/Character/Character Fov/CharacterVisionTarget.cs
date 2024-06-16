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
    
    public float Visibility = 1f;
    public Character Character { get; private set; }
    
    private void Start()
    {
        Character = GetComponentInParent<Character>();
        if (Character.CharacterHealth is CharacterHealth health)
        {
            health.EventDamageDeclined += OnDamageDeclined;
        }
        
        _targets.Add(this);
    }

    private void OnDestroy()
    {
        _targets.Remove(this);
    }
    
    private void OnDamageDeclined()
    {
        Visibility = 0f;
        DOVirtual.DelayedCall(delayOnDamageDecline, () => Visibility = 1f);
    }
}
