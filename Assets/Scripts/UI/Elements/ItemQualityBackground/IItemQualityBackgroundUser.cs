using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemQualityBackgroundUser
{
    public ThiefItem Item { get; }
    
    public event Action EventItemChanged;
}
