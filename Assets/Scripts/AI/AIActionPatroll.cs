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
    [SerializeField] private float distanceOnSpline;

    private Transform _target;
    private SplineComputer _spline;

    public override void Initialization()
    {
        base.Initialization();

        _spline = GetComponentInParent<SplineComputer>();

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
        if (_spline)
        {
            Vector3 from = _brain.Owner.transform.position;
            _target.position = _spline.EvaluatePosition(
                _spline.Travel(
                    _spline.Project(from).percent, distanceOnSpline));
        }
    }
}
