using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class ThiefLevel : LevelProcessor
{
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
    }

    [ContextMenu("Build Navmesh")]
    private void BuildNavmesh()
    {
        NavMeshSurface surface = GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }
}
