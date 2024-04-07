using System.Collections;
using System.Collections.Generic;
using Larje.Core.Tools.TopDownEngine;
using UnityEngine;

public class CharacterStepSound : MonoBehaviour
{
    [SerializeField] private SoundTransmitter soundPrefab;

    private ActualSpeedCharacterMovement _movement;
    
    private void Start()
    {
        _movement = GetComponent<ActualSpeedCharacterMovement>();
        AnimatorEventReceiver receiver = GetComponentInChildren<AnimatorEventReceiver>();
        if (receiver)
        {
            receiver.EventAnimation += OnAnimationEvent;
        }
    }

    private void OnAnimationEvent(string eventName)
    {
        if (eventName == "Step")
        {
            SoundTransmitter sound = Instantiate(soundPrefab, transform);
            sound.transform.localPosition = Vector3.zero;
            sound.Init(5f * _movement.ActualSpeedPercent);
        }
    }
}
