using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DG.Tweening.Plugins.Core.PathCore;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.AI;
using Color = UnityEngine.Color;

public class EnemySeek : CharacterAbility
{
    [SerializeField] private EnemySeekConfig config;

    private float _lastUpdateTime;
    private Vector3 _lastAttentionPoint;
    private CharacterFOV _fov;
    private EnemyAttention _attention;
    private ThiefLevel _level;
    
    private List<SeekPoint> _seekPoints;

    public override void ProcessAbility()
    {
        base.ProcessAbility();

        if (!AbilityAuthorized || !AbilityPermitted)
        {
            _seekPoints = null;
            
            return;
        }
        
        float deltaTime = Time.time - _lastUpdateTime;
        
        TryBuildPoints();
        ObservePoints(deltaTime);
        
        _lastUpdateTime = Time.time;
    }

    public bool TryFindBestPoint(out Vector3 point, out bool isPointVisible)
    {
        point = Vector3.zero;
        isPointVisible = false;
        
        if (_seekPoints == null || _seekPoints.Count == 0)
        {
            return false;
        }

        float maxDistance = 0f;
        Dictionary<SeekPoint, NavMeshPath> paths = new Dictionary<SeekPoint, NavMeshPath>();
        foreach (SeekPoint seekPoint in _seekPoints)
        {
            NavMeshPath path = GetPathToPoint(seekPoint.Position);
            if (path != null)
            {
                paths.Add(seekPoint, path);
                maxDistance = Mathf.Max(maxDistance, path.GetLength());
            }
        }

        List<SeekPoint> availablePoints = _seekPoints.FindAll(x => paths.ContainsKey(x));
        if (availablePoints.Count > 0)
        {
            SeekPoint bestPoint = availablePoints.OrderByDescending(p =>
            {
                float currentDistance = 1f - (paths[p].GetLength() / maxDistance);
                return p.GetPriority() * currentDistance;
            }).First();
        
            point = bestPoint.Position;
            isPointVisible = paths[bestPoint].corners.Length <= 2 && 
                             paths[bestPoint].GetLength() <= config.MaxSeekDistance / 2f;
        
            return true;   
        }
        else
        {
            Debug.LogWarning("Seek points count is 0");
        }

        return false;
    }

    protected override void Initialization()
    {
        base.Initialization();

        _lastUpdateTime = Time.time;
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
    
    private void ObservePoints(float deltaTime)
    {
        if (_seekPoints != null)
        {
            foreach (SeekPoint point in _seekPoints.ToArray())
            {
                float distance = Vector3.Distance(transform.position, point.Position);
                float distanceModifier = Mathf.Clamp01(1f - (distance / config.MaxObserveDistance));
                
                bool observable = _fov.PointsInVision.Contains(point.Position);
                observable |= distance <= config.ForceObserveDistance;
                observable &= distanceModifier > 0f;
                
                if (observable)
                {
                    point.Observe(deltaTime * config.PointObserveSpeed * distanceModifier);
                }
                else
                {
                    point.Recover(deltaTime * config.PointRecoverySpeed);
                }
            }
        }
    }

    private NavMeshPath GetPathToPoint(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        Vector3 sourcePosition = transform.position;
        if (NavMesh.CalculatePath(sourcePosition, targetPosition, NavMesh.AllAreas, path) 
            /*&& path.IsAvailable(targetPosition)*/)
        {
            return path;
        }
        
        return null;
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

        public void Recover(float delta)
        {
            _lookPriority = Mathf.Clamp01(_lookPriority + delta);
        }
        
        public void Observe(float delta)
        {
            _lookPriority = Mathf.Clamp01(_lookPriority - delta);
        }

        public float GetPriority()
        {
            return _distancePriority * _lookPriority;
        }
    }
}
