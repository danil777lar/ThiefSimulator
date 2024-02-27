using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterCarry3D : CharacterAbility
{
    [SerializeField] private LayerMask cargosMask;
    [SerializeField] private float findDistance;
    [SerializeField] private Joint cargoPullPoint; 
    [Header("Gizmos")]
    [SerializeField] private bool drawGizmos;
    [SerializeField] private Color gizmosColor;
    
    private Cargo _nearestCargo;
    private Cargo _cargo;
    
    protected override void Initialization()
    {
        base.Initialization();
    }

    public override void ProcessAbility()
    {
        base.ProcessAbility();
        if (_cargo == null)
        {
            TryFindCargo();
        }
    }

    private void FixedUpdate()
    {
        if (_cargo)
        {
            cargoPullPoint.transform.localPosition = Vector3.zero;
            cargoPullPoint.transform.LookAt(_cargo.AttachPoint.position);
            cargoPullPoint.transform.position += cargoPullPoint.transform.forward * findDistance;
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_nearestCargo != null && _cargo == null)
            {
                AttachCargo(_nearestCargo);   
            }
            else if (_cargo != null)
            {
                DetachCargo();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = gizmosColor;
            Gizmos.DrawWireSphere(transform.position, findDistance);

            if (_nearestCargo != null)
            {
                Gizmos.DrawSphere(_nearestCargo.NearestAttachToPoint(transform.position).position, 0.2f);
            }
        }   
    }

    private void TryFindCargo()
    {
        List<Cargo> cargos = new List<Cargo>();
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, findDistance,cargosMask);
        foreach (Collider cargoCollider in  colliders)
        {
            if (cargoCollider.attachedRigidbody != null && cargoCollider.attachedRigidbody.TryGetComponent(out Cargo cargo))
            {
                cargos.Add(cargo);
            }
        }

        if (cargos.Count > 0)
        {
            _nearestCargo = cargos.OrderBy(x => 
                Vector3.Distance(transform.position, x.NearestAttachToPoint(transform.position).position))
                .First();
        }
        else
        {
            _nearestCargo = null;
        }
    }

    private void AttachCargo(Cargo cargo)
    {
        _cargo = cargo;
        
        Transform attachPoint = _cargo.NearestAttachToPoint(transform.position);
        
        cargoPullPoint.connectedBody = _cargo.Rigidbody;
        cargoPullPoint.autoConfigureConnectedAnchor = false;
        cargoPullPoint.connectedAnchor = _cargo.transform.InverseTransformPoint(attachPoint.position);
        
        _cargo.Attach(attachPoint);
    }
    
    private void DetachCargo()
    {
        _cargo.Detach();
        _cargo = null;
        cargoPullPoint.connectedBody = null;
    }
}





















