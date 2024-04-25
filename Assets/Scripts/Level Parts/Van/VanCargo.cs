using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Tools;
using UnityEngine;

public class VanCargo : MonoBehaviour
{
    [SerializeField] private Transform cargoRoot;
    [SerializeField] private List<Doors> doors;
    
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
        
        foreach (Doors door in doors)
        {
            Vector3 rotation = door.Door.localRotation.eulerAngles.MMSetY(door.OpenAngle);
            door.Door.DOLocalRotate(rotation, 0.5f).SetTarget(this);
        }
        cargoRoot.DOScale(1f, 0.5f).SetTarget(this);
    }
    
    private void CloseDoors()
    {
        this.DOKill();
        
        foreach (Doors door in doors)
        {
            Vector3 rotation = door.Door.localRotation.eulerAngles.MMSetY(0f);
            door.Door.DOLocalRotate(rotation, 0.5f).SetTarget(this);
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
