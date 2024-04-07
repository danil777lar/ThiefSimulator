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
            EventSoundReceived?.Invoke(1f, other.transform.position);
        }
    }
}
