using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Tools.TopDownEngine;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AIActionAimToNearestSeekPoint : AIAction
{
    [SerializeField] private float lookSpeed;
    [SerializeField] private float recalculatePointDelayMin = 1f;
    [SerializeField] private float recalculatePointDelayMax = 5f;
    [Header("Gizmos")]
    [SerializeField] private bool drawGizmos;

    private bool _canUpdatePoint = true;
    private bool _isPointVisible;
    private Transform _target;
    private Transform _targetLook;
    private EnemySeek _seek;
    private Vector3 _point;
    private CharacterFOV _fov;
    private CoreCharacterOrientation3D _orientation;
    
    public override void Initialization()
    {
        base.Initialization();

        _seek = _brain.Owner.GetComponentInChildren<EnemySeek>();
        _orientation = _brain.Owner.GetComponentInChildren<CoreCharacterOrientation3D>();
        _fov = _brain.Owner.GetComponentInChildren<CharacterFOV>();
        
        _target = new GameObject().transform;
        _target.gameObject.name = "Seek Point Target";
        _target.SetParent(transform);
        
        _targetLook = new GameObject().transform;
        _targetLook.gameObject.name = "Seek Point Target Look";
        _targetLook.SetParent(transform);
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        _canUpdatePoint = true;
        _brain.Target = _target;
        _orientation.forceLookTarget = _targetLook;
    }
    
    public override void OnExitState()
    {
        base.OnExitState();
        _brain.Target = null;
        _orientation.forceLookTarget = null;
        
        StopAllCoroutines();
    }

    public override void PerformAction()
    {
        if (_canUpdatePoint)
        {
            if (_seek.TryFindBestPoint(out Vector3 point))
            {
                _point = point;
            }
            StartCoroutine(RecalculatePointDelay());
        }
        
        _isPointVisible = _fov.IsPointInVision(_point);
        
        Vector3 lookPoint = _isPointVisible ? _point : transform.position + _orientation.TargetDirection * 10f;
        _target.position = _isPointVisible ? transform.position : _point;
        _targetLook.position = Vector3.Lerp(_targetLook.position, lookPoint, Time.deltaTime * lookSpeed);
    }

    private void OnDisable()
    {
        _orientation.forceLookTarget = null;
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = _isPointVisible ? Color.green : Color.blue;
            Gizmos.DrawSphere(_point, 0.5f);
        }
    }

    private IEnumerator RecalculatePointDelay()
    {
        _canUpdatePoint = false;
        yield return new WaitForSeconds(Random.Range(recalculatePointDelayMin, recalculatePointDelayMax));
        _canUpdatePoint = true;
    }
}
