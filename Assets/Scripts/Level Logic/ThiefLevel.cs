using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using MoreMountains.TopDownEngine;
using Unity.AI.Navigation;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class ThiefLevel : LevelProcessor
{
    [Header("Grid")]
    [SerializeField, Min(1f)] private float gridSize = 2f;
    [SerializeField, Min(1f)] private float maxPointDistance = 1f;
    [Header("Gizmos")] 
    [SerializeField] private Color gizmoColor;
    [SerializeField] private float gizmoSize;

    public IReadOnlyList<Vector3> Points;
    public IReadOnlyList<Character> Characters;
    
    public override void TryStartLevel(StartData data)
    {
        StartLevel(data);
    }

    public override void TryStopLevel(StopData data)
    {
        StopLevel(data);
    }

    public override LevelData GetLevelData()
    {
        return null;
    }
    
    private void Start()
    {
        BuildNavmesh();
        GrabCharacters();
    }

    private void OnDrawGizmos()
    {
        if (Points != null)
        {
            Gizmos.color = gizmoColor;
            foreach (Vector3 point in Points)
            {
                Gizmos.DrawSphere(point, gizmoSize);   
            }
        }
    }

    [ContextMenu("Build Navmesh")]
    private void BuildNavmesh()
    {
        NavMeshSurface surface = GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();

        List<Vector3> points = new List<Vector3>();
        for (float x = -100f; x <= 100f; x += gridSize)
        {
            for (float z = -100f; z <= 100f; z += gridSize)
            {
                Vector3 point = transform.position + new Vector3(x, 0, z);
                if (NavMesh.SamplePosition(point, out NavMeshHit hit, maxPointDistance, NavMesh.AllAreas))
                {
                    points.Add(hit.position);
                }
            }   
        }

        Points = points.AsReadOnly();
    }

    private void GrabCharacters()
    {
        Characters = GetComponentsInChildren<Character>();
    }
}
