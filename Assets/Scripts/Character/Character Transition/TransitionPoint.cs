using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    [SerializeField] private float duration;
    [Space]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    private SplineComputer _spline;

    public float Duration => duration;

    public bool TryGetStartAndEndPercent(Transform point, out float start, out float end)
    {
        start = 0f;
        end = 0f;
        
        if (point == pointA)
        {
            end = 1f;
            return true;
        }
        
        if (point == pointB)
        {
            start = 1f;
            return true;
        }
        
        return false;
    }

    public Vector3 EvaluatePosition(float percent)
    {
        return _spline.EvaluatePosition(percent);
    } 
    
    private void Start()
    {
        _spline = GetComponent<SplineComputer>();

        pointA.position = _spline.EvaluatePosition(0f);
        pointB.position = _spline.EvaluatePosition(1f);
    }
}
