using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundReceiver : MonoBehaviour
{
    [SerializeField] private LayerMask soundMask;

    public event Action<float, Vector3> EventSoundReceived;

    private void OnTriggerEnter(Collider other)
    {
        if (soundMask.HasLayer(other.gameObject.layer))
        {
            SoundTransmitter sound = other.GetComponentInParent<SoundTransmitter>();
            if (sound != null)
            {
                EventSoundReceived?.Invoke(sound.CurrentAmplitude, other.transform.position);
            }
        }
    }
}
