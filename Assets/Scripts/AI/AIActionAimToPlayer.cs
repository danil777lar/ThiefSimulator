using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.AI;

public class AIActionAimToPlayer : AIAction
{
    [SerializeField] private Transform target;
    [SerializeField] private float minDistance;
    [SerializeField] private float rotationSpeed;

    private float _lastPerformTime;
    private Vector3 _currentDirection;
    private NavMeshPath _path;
    private Character _character;
    private Character _player;

    public override void Initialization()
    {
        base.Initialization();
        _path = new NavMeshPath();
        _character = _brain.Owner.GetComponent<Character>();
        _player = FindObjectsByType<Character>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .ToList().Find(x => x.CharacterType == Character.CharacterTypes.Player);

        _currentDirection = _character.CharacterModel.transform.forward;
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        _brain.Target = target;
        _lastPerformTime = Time.time;
    }

    public override void PerformAction()
    {
        float deltaTime = Time.time - _lastPerformTime;
        _lastPerformTime = Time.time;
        
        if (_player)
        {
            Vector3 from = transform.position;
            Vector3 to = _player.transform.position;
            if (NavMesh.CalculatePath(from, to, NavMesh.AllAreas, _path))
            {
                foreach (Vector3 corner in _path.corners)
                {
                    target.position = corner;
                    if (Vector3.Distance(target.position, transform.position) >= minDistance)
                    {
                        break;
                    }
                }
            }

            Vector3 direction = target.position - _character.transform.position;
            _currentDirection = Vector3.RotateTowards(_currentDirection, direction,
                Mathf.Deg2Rad * rotationSpeed * deltaTime, 0f).normalized; 
            target.position = _character.transform.position + (_currentDirection * direction.magnitude);
        }
    }

    private void OnDrawGizmos()
    {
        if (_path != null && _path.corners.Length > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _path.corners[0]);
            for (int i = 0; i < _path.corners.Length - 1; i++)
            {
                Gizmos.DrawSphere(_path.corners[i], 0.25f);
                Gizmos.DrawLine(_path.corners[i], _path.corners[i + 1]);
            }
            Gizmos.DrawSphere(_path.corners[^1], 0.25f);
        }
    }
}
