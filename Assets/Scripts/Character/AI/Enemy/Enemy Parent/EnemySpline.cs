using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using MoreMountains.TopDownEngine;
using UnityEngine;

[RequireComponent(typeof(SplineComputer))]
public class EnemySpline : MonoBehaviour, IEnemyPatrolProcessor
{
    [SerializeField] private bool directionForward = true;
    [SerializeField] private float distanceOnSpline = 2f;

    private Character _enemy;
    private SplineComputer _spline;

    public bool TryGetPosition(out Vector3 position)
    {
        position = Vector3.zero;
        
        if (!_spline)
        {
            return false;
        }

        double percentTravel = _spline.Travel(0f, distanceOnSpline);
        double targetPercent = _spline.Project(_enemy.transform.position).percent;
        targetPercent += percentTravel * (directionForward ? 1f : -1f);

        if (targetPercent > 1f)
        {
            if (_spline.isClosed)
            {
                targetPercent -= 1f;
            }
            else
            {
                directionForward = !directionForward;
            }
        }
        else if (targetPercent < 0f)
        {
            if (_spline.isClosed)
            {
                targetPercent += 1f;
            }
            else
            {
                directionForward = !directionForward;
            }
        }

        position = _spline.EvaluatePosition(Mathf.Clamp01((float)targetPercent));
        return true;
    }

    public bool TryGetLookPosition(out Vector3 direction)
    {
        direction = Vector3.zero;
        return false;
    }
    
    private void Start()
    {
        _spline = GetComponent<SplineComputer>();
        _enemy = GetComponentInChildren<Character>();
    }
}
