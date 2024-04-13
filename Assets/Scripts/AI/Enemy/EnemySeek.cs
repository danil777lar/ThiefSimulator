using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.AI;
using Color = UnityEngine.Color;

public class EnemySeek : CharacterAbility
{
    [SerializeField] private EnemySeekConfig config;
    
    private Vector3 _lastAttentionPoint;
    private CharacterFOV _fov;
    private EnemyAttention _attention;
    private ThiefLevel _level;
    
    private List<SeekPoint> _seekPoints;

    public bool TryFindBestPoint(out Vector3 point)
    {
        if (_seekPoints == null || _seekPoints.Count == 0)
        {
            point = Vector3.zero;
            return false;
        }
        
        SeekPoint bestPoint = _seekPoints.OrderByDescending(p => p.GetPriority()).First();
        point = bestPoint.Position;
        return true;
    }
    
    public override void ProcessAbility()
    {
        base.ProcessAbility();
        
        TryBuildPoints();
        LookToPoints();
        UpdatePoints();
    }
    
    protected override void Initialization()
    {
        base.Initialization();
        
        _fov = GetComponent<CharacterFOV>();
        _attention = GetComponent<EnemyAttention>();
        _level = GetComponentInParent<ThiefLevel>();
    }
    
    private void OnDrawGizmos()
    {
        if (_seekPoints != null)
        {
            foreach (SeekPoint point in _seekPoints)
            {
                Gizmos.color = Color.red.SetAlpha(point.GetPriority());
                Gizmos.DrawSphere(point.Position, 0.4f);
            }
        }
    }
    
    private void UpdatePoints()
    {
        if (_seekPoints != null)
        {
            foreach (SeekPoint point in _seekPoints)
            {
                point.Update(Time.deltaTime);
            }
        }
    }

    private void TryBuildPoints()
    {
        if (_lastAttentionPoint != _attention.LastAttentionPoint)
        {
            _lastAttentionPoint = _attention.LastAttentionPoint;
            _seekPoints = new List<SeekPoint>();
            foreach (Vector3 levelPoint in _level.Points)
            {
                NavMeshPath path = new NavMeshPath();
                Vector3 sourcePosition = _lastAttentionPoint;
                Vector3 targetPosition = levelPoint;
                if (NavMesh.CalculatePath(sourcePosition, targetPosition, NavMesh.AllAreas, path))
                {
                    if (path.IsAvailable(targetPosition) && path.GetLength() <= config.MaxSeekDistance)
                    {
                        float distance = path.GetLength() / config.MaxSeekDistance;
                        _seekPoints.Add(new SeekPoint(targetPosition, distance));
                    }
                }
            }
        }
    }
    
    private void LookToPoints()
    {
        if (_seekPoints != null)
        {
            foreach (SeekPoint point in _seekPoints.ToArray())
            {
                if (_fov.PointsInVision.Contains(point.Position))
                {
                    point.LookAtPoint();
                }
            }
        }
    }
    
    private class SeekPoint
    {
        public Vector3 Position { get; }
        
        private float _distancePriority;
        private float _lookPriority;
        
        public SeekPoint(Vector3 position, float distance)
        {
            Position = position;
            
            _distancePriority = Mathf.Clamp01(1f - distance);
            _lookPriority = 1f;
        }

        public void Update(float deltaTime)
        {
            _lookPriority = Mathf.Clamp01(_lookPriority + deltaTime);
        }
        
        public void LookAtPoint()
        {
            _lookPriority = 0f;
        }

        public float GetPriority()
        {
            return _distancePriority * _lookPriority;
        }
    }
}
