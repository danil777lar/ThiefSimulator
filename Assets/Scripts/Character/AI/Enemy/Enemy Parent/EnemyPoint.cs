using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class EnemyPoint : MonoBehaviour, IEnemyPatrolProcessor
{
    [SerializeField] private float distanceToRotate = 1f;
    [SerializeField] private List<RotationStep> rotationSteps;

    private int _currentStep;
    private float _currentAngle;
    private Character _character;
    
    public bool TryGetPosition(out Vector3 position)
    {
        position = transform.position;
        
        return true;
    }

    public bool TryGetLookPosition(out Vector3 lookPosition)
    {
        if (Vector3.Distance(_character.transform.position, transform.position) <= distanceToRotate)
        {
            lookPosition = transform.position + AnglesToDirection(_currentAngle);
            return true;   
        }
        
        lookPosition = transform.position;
        return false;
    }
    
    private void Start()
    {
        _character = GetComponentInChildren<Character>();
        StartStep();
    }

    private Vector3 AnglesToDirection(float angle)
    {
        return Quaternion.Euler(0, angle, 0) * transform.forward;
    }

    private void StartStep()
    {
        if (rotationSteps is { Count: > 0 })
        {
            RotationStep step = rotationSteps[_currentStep % rotationSteps.Count];
            
            float angleFrom = _currentAngle;
            float targetAngle = _currentAngle + step.AngleOffset;

            Sequence sequence = DOTween.Sequence();
            
            sequence.Append(DOTween.To(
                () => angleFrom,
                (x) => _currentAngle = x,
                targetAngle, step.Duration));
            
            sequence.AppendInterval(step.EndDelay);

            sequence.AppendCallback(() =>
            {
                _currentStep++;
                StartStep();
            });
        }
    } 

    [Serializable]
    private class RotationStep
    {
        [field: SerializeField] public float AngleOffset { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public float EndDelay { get; private set; }
    }
}
