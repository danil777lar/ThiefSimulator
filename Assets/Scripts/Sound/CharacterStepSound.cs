using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Tools.TopDownEngine;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterStepSound : MonoBehaviour
{
    [SerializeField] private SoundTransmitter soundPrefab;
    [Header("Amplitude")]
    [SerializeField] private float baseAmplitude;
    [SerializeField] private float minAmplitude;
    [SerializeField] private AnimationCurve amplitudeBySpeed;
    [Header("By Distance")]
    [SerializeField] private bool spawnByDistance;
    [SerializeField] private float distanceBetweenSounds;

    private float _traveledDistance;
    private Vector3 _lastPosition;
    private CoreCharacterMovement _movement;
    
    private List<Func<float>> _amplitudeMultipliers = new List<Func<float>>();
    
    public void TryAddAmplitudeMultiplier(Func<float> multiplier)
    {
        if (!_amplitudeMultipliers.Contains(multiplier))
        {
            _amplitudeMultipliers.Add(multiplier);
        }
    }
    
    public void TryRemoveAmplitudeMultiplier(Func<float> multiplier)
    {
        if (_amplitudeMultipliers.Contains(multiplier))
        {
            _amplitudeMultipliers.Remove(multiplier);
        }
    }
    
    private void Start()
    {
        _lastPosition = transform.position;
        _movement = GetComponentInParent<CoreCharacterMovement>();

        if (!spawnByDistance)
        {
            AnimatorEventReceiver receiver = _movement.GetComponentInChildren<AnimatorEventReceiver>();
            if (receiver)
            {
                receiver.EventAnimation += OnAnimationEvent;
            }   
        }
    }

    private void Update()
    {
        if (spawnByDistance)
        {
            _traveledDistance += Vector3.Distance(transform.position, _lastPosition);
            if (_traveledDistance >= distanceBetweenSounds)
            {
                _traveledDistance = 0f;
                SpawnSound();
            }
        }

        _lastPosition = transform.position;
    }

    private void OnAnimationEvent(string eventName)
    {
        if (eventName == "Step")
        {
            SpawnSound();
        }
    }

    private void SpawnSound()
    {
        if (_movement.AbilityAuthorized && _movement.AbilityPermitted)
        {
            float amplitude = baseAmplitude * amplitudeBySpeed.Evaluate(_movement.ActualSpeedPercent);
            amplitude *= GetAmplitudeMultiplier();
            
            if (amplitude < minAmplitude)
            {
                return;
            }

            SoundTransmitter sound = Instantiate(soundPrefab, transform);
            sound.transform.localPosition = Vector3.zero;
            sound.Init(amplitude);
        }
    }
    
    private float GetAmplitudeMultiplier()
    {
        float result = 1f;
        foreach (Func<float> multiplier in _amplitudeMultipliers)
        {
            result *= multiplier.Invoke();
        }

        return result;
    }
}
