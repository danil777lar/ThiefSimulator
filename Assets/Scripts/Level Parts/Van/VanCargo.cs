using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class VanCargo : MonoBehaviour
{
    [SerializeField] private Transform cargoRoot;
    [SerializeField] private List<Doors> doors;
    [Space]
    [SerializeField] private SoundSettings doorSound;
    
    private bool _playSound = false;
    private VanMovement _vanMovement;
    
    private void Start()
    {
        _vanMovement = GetComponentInParent<VanMovement>();
        _vanMovement.EventStartMove += CloseDoors;
        _vanMovement.EventStopMove += OpenDoors;
    }
    
    private void OpenDoors()
    {
        this.DOKill();
        
        doorSound.Play(s => s.SetTarget(this).SetPosition(t => transform.position).SetSpatialBlend(t => 1f));
        foreach (Doors door in doors)
        {
            Vector3 rotation = door.Door.localRotation.eulerAngles;
            rotation.y = door.OpenAngle;
            door.Door.DOLocalRotate(rotation, 0.5f).SetTarget(this);
        }
        cargoRoot.DOScale(1f, 0.5f).SetTarget(this);
    }
    
    private void CloseDoors()
    {
        this.DOKill();

        if (_playSound) doorSound.Play(s => s.SetTarget(this).SetPosition(t => transform.position).SetSpatialBlend(t => 1f));
        _playSound = true;

        foreach (Doors door in doors)
        {
            Vector3 rotation = door.Door.localRotation.eulerAngles;
            rotation.y = 0f;
            door.Door.DOLocalRotate(rotation, 0.5f).SetTarget(this).OnComplete(() =>
            {
            });
        }
        cargoRoot.DOScale(0f, 0.5f).SetTarget(this);
    }

    [Serializable]
    private class Doors
    {
        [field: SerializeField] public Transform Door { get; private set; }
        [field: SerializeField] public float OpenAngle { get; private set; }
    }
}
