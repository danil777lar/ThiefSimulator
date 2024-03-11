using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DoorOpenClose : MonoBehaviour
{
    [SerializeField] private bool isActiveOnStart;
    [SerializeField] private float doorOpenAngle;
    [Space]
    [SerializeField] private LayerMask interactionMask;
    [Space]
    [SerializeField] private GameObject trigger;
    [SerializeField] private Transform[] doorRoots;
    [Space]
    [SerializeField] private float openDuration = 0.5f;
    [SerializeField] private Ease openEase = Ease.OutBack;
    [Space]
    [SerializeField] private float closeDuration = 5f;
    [SerializeField] private Ease closeEase = Ease.OutElastic;

    private bool _isActive;
    private List<Collider> _interactions = new List<Collider>();

    public void SetIsActive(bool arg)
    {
        _isActive = arg;
        trigger.SetActive(arg);

        if (arg)
        {
            if (_interactions.Count > 0)
            {
                OpenDoor(_interactions[0].transform.position);
            }
            else
            {
                CloseDoor();
            }
        }
    }
    
    private void Start()
    {
        SetIsActive(isActiveOnStart);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (interactionMask.HasLayer(other.gameObject.layer))
        {
            if (!_interactions.Contains(other))
            {
                if (_interactions.Count == 0)
                {
                    OpenDoor(other.transform.position);
                }
                _interactions.Add(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (interactionMask.HasLayer(other.gameObject.layer))
        {
            if (_interactions.Contains(other))
            {
                _interactions.Remove(other);
                if (_interactions.Count == 0)
                {
                    CloseDoor();
                }
            }
        }
    }

    private void OpenDoor(Vector3 position)
    {
        if (_isActive)
        {
            this.DOKill();
            float direction = transform.InverseTransformPoint(position).x < 0f ? 1f : -1f;
            foreach (Transform door in doorRoots)
            {
                door.DOLocalRotate(Vector3.up * 90f * direction, openDuration)
                    .SetTarget(this)
                    .SetEase(openEase);
            }
        }
    }

    private void CloseDoor()
    {
        if (_isActive)
        {
            this.DOKill();
            foreach (Transform door in doorRoots)
            {
                door.DOLocalRotate(Vector3.zero, closeDuration)
                    .SetTarget(this)
                    .SetEase(closeEase);
            }
        }
    }
}
