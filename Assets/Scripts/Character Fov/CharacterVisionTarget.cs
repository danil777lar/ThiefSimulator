using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterVisionTarget : MonoBehaviour
{
    private static List<CharacterVisionTarget> _targets = new List<CharacterVisionTarget>();
    public static IReadOnlyCollection<CharacterVisionTarget> Targets => _targets.AsReadOnly();
    
    public float Visibility = 1f;
    public Character Character { get; private set; }
    
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
