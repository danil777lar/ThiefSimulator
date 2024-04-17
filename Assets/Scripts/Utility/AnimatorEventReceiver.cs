using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEventReceiver : MonoBehaviour
{
    public event Action<string> EventAnimation;
    public event Action EventSendDamage;
    public event Action EventAttackComplete;
    
    private void AnimationEvent(string eventName)
    {
        EventAnimation?.Invoke(eventName);
    }

    private void SendDamageEvent()
    {
        EventSendDamage?.Invoke();
    }

    private void AttackCompleteEvent()
    {
        EventAttackComplete?.Invoke();
    }
}
