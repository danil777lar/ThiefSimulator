using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.AI;

public class AIActionPatroll : AIAction
{
    [SerializeField] private bool directionForward = true;
    [SerializeField] private float distanceOnSpline;

    private Vector3 _defaultPosition;
    private Transform _target;
    private SplineComputer _spline;

    public override void Initialization()
    {
        base.Initialization();

        _spline = GetComponentInParent<SplineComputer>();

        _defaultPosition = transform.position;

        _target = new GameObject().transform;
        _target.gameObject.name = "Patroll Target";
        _target.SetParent(transform);
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        _brain.Target = _target;
    }

    public override void PerformAction()
    {
        if (!_spline)
        {
            _target.position = _defaultPosition;
            return;
        }

        double percentTravel = _spline.Travel(0f, distanceOnSpline);
        double targetPercent = _spline.Project(_brain.Owner.transform.position).percent;
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

        _target.position = _spline.EvaluatePosition(Mathf.Clamp01((float)targetPercent));
    }
}
