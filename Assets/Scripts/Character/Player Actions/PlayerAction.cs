using System;
using UnityEngine;

[Serializable]
public class PlayerAction
{
    public readonly Action Action;
    public readonly Func<bool> Enabled;
    public readonly Func<float> Duration;
    public readonly Sprite Icon;

    public PlayerAction(Action action, Func<bool> enabled, Func<float> duration, Sprite icon)
    {
        Action = action;
        Enabled = enabled;
        Duration = duration;
        Icon = icon;
    }
}
