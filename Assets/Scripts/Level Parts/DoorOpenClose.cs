using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Dreamteck;
using UnityEngine;

public class DoorOpenClose : MonoBehaviour
{
    [SerializeField] private bool isActiveOnStart;
    [SerializeField] private float doorOpenAngle;
    [Space]
    [SerializeField] private LayerMask interactionMask;
    [Space]
    [SerializeField] private GameObject trigger;
    [SerializeField] private DoorRoot[] doorRoots;
    [Space]
    [SerializeField] private float openDuration = 0.5f;
    [SerializeField] private Ease openEase = Ease.OutBack;
    [Space]
    [SerializeField] private float closeDuration = 5f;
    [SerializeField] private Ease closeEase = Ease.OutElastic;

    private bool _isActive;
    private bool _doorOpened;
    private List<Collider> _doorColliders;
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

        _doorColliders = new List<Collider>();
        doorRoots.ForEach(x => _doorColliders.AddRange(x.Root.GetComponentsInChildren<Collider>()));
    }

    private void Update()
    {
        _doorColliders.ForEach(x => x.isTrigger = _doorOpened && _isActive);
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
            _doorOpened = true;
                
            this.DOKill();
            float direction = transform.InverseTransformPoint(position).x < 0f ? 1f : -1f;
            foreach (DoorRoot door in doorRoots)
            {
                door.Root.DOLocalRotate(Vector3.up * 90f * direction * door.AngleMultiplier, openDuration)
                    .SetTarget(this)
                    .SetEase(openEase);
            }
        }
    }

    private void CloseDoor()
    {
        if (_isActive)
        {
            _doorOpened = false;
            
            this.DOKill();
            foreach (DoorRoot door in doorRoots)
            {
                door.Root.DOLocalRotate(Vector3.zero, closeDuration)
                    .SetTarget(this)
                    .SetEase(closeEase);
            }
        }
    }

    [Serializable]
    private class DoorRoot
    {
        [field: SerializeField] public Transform Root { get; private set; }
        [field: SerializeField] public float AngleMultiplier { get; private set; }
    }
}
