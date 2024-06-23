using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Quality Background Config", menuName = "Configs/Item Quality Background Config")]
public class ItemQualityBackgroundConfig : ScriptableObject
{
    [SerializeField] private List<ItemBackground> backgrounds;
    
    public Sprite GetBackground(ThiefItem item)
    {
        if (item == null) return null;
        
        return backgrounds.Find(bg => bg.Quality == item.Quality)?.Background;
    }
    
    [Serializable]
    public class ItemBackground
    {
        [field: SerializeField] public ItemQuality Quality { get; private set; }
        [field: SerializeField] public Sprite Background { get; private set; }
    }
}
