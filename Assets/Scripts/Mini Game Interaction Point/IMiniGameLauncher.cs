using System;
using System.Collections;
using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;

public interface IMiniGameLauncher
{
    public MiniGameType MiniGameType { get; }

    public void AddMultiplier(Func<float> multiplier);
    public void RemoveMultiplier(Func<float> multiplier);
}
