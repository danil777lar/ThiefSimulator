using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Character;
using Larje.Character.AI;
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
    private Vector3 _point;
    private Transform _target;
    private Transform _targetLook;
    private EnemySeek _seek;
    private CharacterFOV _fov;
    private CharacterOrientation _orientation;
    
    protected override void OnInitialized()
    {
        base.Initialization();

        _seek = Brain.Owner.GetComponentInChildren<EnemySeek>();
        _orientation = Brain.Owner.GetComponentInChildren<CharacterOrientation>();
        _fov = Brain.Owner.GetComponentInChildren<CharacterFOV>();
        
        _target = new GameObject().transform;
        _target.gameObject.name = "Seek Point Target";
        _target.SetParent(transform);
        
        _targetLook = new GameObject().transform;
        _targetLook.gameObject.name = "Seek Point Target Look";
        _targetLook.SetParent(transform);
    }

    protected override void OnEnterState()
    {
        base.OnEnterState();
        _canUpdatePoint = true;
        Brain.Target = _target;

        // _orientation.forceLookTarget = _targetLook;
    }
    
    protected override void OnExitState()
    {
        base.OnExitState();
        Brain.Target = null;

        // _orientation.forceLookTarget = null;
        
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

        Vector3 targetDirection = Vector3.forward; //_orientation.TargetDirection;
        Vector3 lookPoint = _isPointVisible ? _point : transform.position + targetDirection * 10f;
        _target.position = _isPointVisible ? transform.position : _point;
        _targetLook.position = Vector3.Lerp(_targetLook.position, lookPoint, Time.deltaTime * lookSpeed);
    }

    private void OnDisable()
    {
        // _orientation.forceLookTarget = null;
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
