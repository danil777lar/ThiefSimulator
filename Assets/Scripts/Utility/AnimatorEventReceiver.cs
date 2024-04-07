using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEventReceiver : MonoBehaviour
{
    public event Action<string> EventAnimation;
    
    private void AnimationEvent(string eventName)
    {
        EventAnimation?.Invoke(eventName);
    }
}
