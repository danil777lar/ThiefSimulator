using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Tools.TopDownEngine;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.AI;

public class AIActionAimToNearestSeekPoint : AIAction
{
    [SerializeField] private float lookSpeed;
    [SerializeField] private float recalculatePointDelayMin = 1f;
    [SerializeField] private float recalculatePointDelayMax = 5f;

    private bool _canUpdatePoint = true;
    private bool _isPointVisible;
    private Transform _target;
    private Transform _targetLook;
    private EnemySeek _seek;
    private Vector3 _point;
    private CoreCharacterOrientation3D _orientation;
    
    public override void Initialization()
    {
        base.Initialization();

        _seek = _brain.Owner.GetComponent<EnemySeek>();
        _orientation = _brain.Owner.GetComponent<CoreCharacterOrientation3D>();
        
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
            if (_seek.TryFindBestPoint(out Vector3 point, out bool isPointVisible))
            {
                _point = point;
                _isPointVisible = isPointVisible;
            }

            _canUpdatePoint = false;
            StartCoroutine(RecalculatePointDelay());
        }
        
        _canUpdatePoint = false;
        Vector3 lookPoint = transform.position + _orientation.Direction * 10f;
        if (_isPointVisible)
        {
            lookPoint = _point;
            _target.position = transform.position;
        }
        else
        {
            _target.position = _point;
        }

        _targetLook.position = Vector3.Lerp(_targetLook.position, lookPoint, Time.deltaTime * lookSpeed);
    }
    
    private IEnumerator RecalculatePointDelay()
    {
        yield return new WaitForSeconds(Random.Range(recalculatePointDelayMin, recalculatePointDelayMax));
        _canUpdatePoint = true;
    }
}
