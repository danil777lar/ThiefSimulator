using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Dreamteck.Splines;
using Larje.Character;
using UnityEngine;

[RequireComponent(typeof(SplineComputer))]
public class EnemySpline : MonoBehaviour, IEnemyPatrolProcessor
{
    [SerializeField] private bool directionForward = true;
    [SerializeField] private float distanceOnSpline = 0.5f;
    [SerializeField] private List<PausePoint> pausePoints;
    
    private double _lastTargetPercent;
    private Character _enemy;
    private PausePoint _pause;
    private SplineComputer _spline;
    
    private SplineComputer Spline 
    {
        get
        {
            if (_spline == null)
            {
                _spline = GetComponent<SplineComputer>();
            }
            return _spline;
        }
    }

    public bool TryGetPosition(out Vector3 position)
    {
        position = Vector3.zero;
        
        if (!Spline)
        {
            return false;
        }

        double percentTravel = Spline.Travel(0f, distanceOnSpline);
        double targetPercent = Spline.Project(_enemy.transform.position).percent;

        if (_pause == null)
        {
            targetPercent += percentTravel * (directionForward ? 1f : -1f);
            
            CheckPausePoint(targetPercent);
            
            _lastTargetPercent = targetPercent;
        }

        if (targetPercent > 1f)
        {
            if (Spline.isClosed)
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
            if (Spline.isClosed)
            {
                targetPercent += 1f;
            }
            else
            {
                directionForward = !directionForward;
            }
        }

        position = Spline.EvaluatePosition(Mathf.Clamp01((float)targetPercent));
        return true;
    }

    private void CheckPausePoint(double targetPercent)
    {
        if (pausePoints == null || pausePoints.Count == 0)
        {
            return;
        }
        
        foreach (PausePoint pausePoint in pausePoints)
        {
            bool isBetween = _lastTargetPercent < pausePoint.Percent && pausePoint.Percent < targetPercent;
            isBetween |= _lastTargetPercent > pausePoint.Percent && pausePoint.Percent > targetPercent;

            if (isBetween)
            {
                _pause = pausePoint;
                DOVirtual.DelayedCall(pausePoint.Delay, () => _pause = null);
                break;
            }
        }
    }

    public bool TryGetLookPosition(out Vector3 lookPoint)
    {
        if (TryGetPauseLookPosition(_pause, out Vector3 pauseLookPoint))
        {
            lookPoint = pauseLookPoint;
            return true;
        }
        
        lookPoint = Vector3.zero;
        return false;
    }
    
    private void Start()
    {
        _enemy = GetComponentInChildren<Character>();
    }

    private void OnDrawGizmos()
    {
        if (pausePoints == null || pausePoints.Count == 0)
        {
            return;
        }
        
        foreach (PausePoint pausePoint in pausePoints)
        {
            Gizmos.color = Color.red;
            Vector3 from = Spline.EvaluatePosition(Mathf.Clamp01((float)pausePoint.Percent));
            Gizmos.DrawSphere(from, 0.35f);
            if (TryGetPauseLookPosition(pausePoint, out Vector3 pauseLookPoint))
            {
                Gizmos.DrawLine(from, pauseLookPoint);
                
                Gizmos.color = Color.red.SetAlpha(0.5f);
                Gizmos.DrawSphere(pauseLookPoint, 0.25f);
            }
        }
    }
    
    private bool TryGetPauseLookPosition(PausePoint point, out Vector3 lookPoint)
    {
        if (point is { UseLookPosition: true })
        {
            Vector3 from = Spline.EvaluatePosition(Mathf.Clamp01((float)point.Percent));

            float angleRad = point.LookAngle * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad));
            
            lookPoint = from + direction * point.LookDistance;
            return true;
        }

        lookPoint = Vector3.zero;
        return false;
    }

    [Serializable]
    private class PausePoint
    {
        [field: SerializeField, Range(0f, 1f)] public double Percent { get; private set; }
        [field: SerializeField] public float Delay { get; private set; }
        [field: Header("Look")]
        [field: SerializeField] public bool UseLookPosition { get; private set; }
        [field: SerializeField, Range(0f, 359f)] public float LookAngle { get; private set; }
        [field: SerializeField, Min(1f)] public float LookDistance { get; private set; } = 2f;
    }
}
