using System;
using System.Collections;
using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;

public abstract class UpgradeProcessor : MonoBehaviour
{
    [field: SerializeField] public UpgradeType UpgradeType { get; private set; }
    [field: SerializeField] public bool Upgradable { get; private set; }
    [field: Header("Display Info")]
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public string DescriptionLeftModifier { get; private set; }
    [field: SerializeField] public string DescriptionRightModifier { get; private set; }
    
    [field: Header("Levels")]
    [field: SerializeField] public LevelCombinationType LevelCombination { get; private set; }
    [field: SerializeField] public float MaxLevel { get; private set; }
    [field: SerializeField] public float BaseValue { get; private set; }
    [field: SerializeField] public float AddValuePerLevel { get; private set; }
    
    [field: Header("Price")]
    [field: SerializeField] public float BaseLevelPrice { get; private set; }
    [field: SerializeField] public float LevelPriceMultiplier { get; private set; }
    
    [Header("Duration")]
    [SerializeField] protected DurationCombinationType durationCombinationType;
    [SerializeField] protected bool unlimitedDuration;
    [SerializeField] protected float duration;

    protected bool _inited;
    protected int _currentLevel;
    protected float _fullDuration;
    protected float _currentDuration;
    
    public event Action EventRemoved;
    
    public string GetDescription(int level)
    {
        //return $"{DescriptionLeftModifier}{GetValueOnLevel(level)}{DescriptionRightModifier}";
        return $"Level {level + 1}";
    }

    public virtual void Init(int level)
    {
        _currentLevel = level;
        _fullDuration = duration;
        _inited = true;
    }

    public virtual void Combine(int level)
    {
        CombineLevel(level);
        CombineDuration();
    }

    public virtual void Remove()
    {
        DestroyImmediate(gameObject);
        EventRemoved?.Invoke();
    }

    protected virtual void Update()
    {
        if (!unlimitedDuration && _inited)
        {
            _currentDuration += Time.deltaTime;
            if (_currentDuration >= _fullDuration)
            {
                Remove();
            }
        }
    }

    protected float GetValueOnLevel(int level)
    {
        return BaseValue + (AddValuePerLevel * level);
    }
    
    protected virtual void CombineLevel(int level)
    {
        if (LevelCombination == LevelCombinationType.Add)
        {
            _currentLevel += level;
        }
    }

    protected virtual void CombineDuration()
    {
        if (durationCombinationType == DurationCombinationType.Reset)
        {
            _currentDuration = 0;
        }
        else if (durationCombinationType == DurationCombinationType.Add)
        {
            _fullDuration += duration;
        }
    }

    public enum DurationCombinationType
    {
        Add,
        Reset,
        Nothing
    }
    
    public enum LevelCombinationType
    {
        Add,
        Nothing
    }
}
