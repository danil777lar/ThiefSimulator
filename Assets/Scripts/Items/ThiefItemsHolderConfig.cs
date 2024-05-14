using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using UnityEngine;

[CreateAssetMenu(fileName = "ThiefItemsHolderConfig", menuName = "Configs/ThiefItemsHolderConfig")]
public class ThiefItemsHolderConfig : ItemsHolderConfig
{
    [SerializeField] private ThiefItem[] items;
    
    public override Item[] Items { get; }
}
