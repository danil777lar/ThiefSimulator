using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using UnityEngine;

[Serializable]
public class ThiefItem : Item
{
    [field: SerializeField] public ItemQuality Quality { get; private set; }
    [field: SerializeField] public string DisplayName { get; private set; }
}
