using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public static class NavMeshPathExtensions
{
    public static float GetLength(this NavMeshPath path)
    {
        float length = 0f;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }

        return length;
    }
    
    public static void DrawDebugPath(this NavMeshPath path, Color color)
    {
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Debug.DrawLine(path.corners[i], path.corners[i + 1], color);
        }
    }
    
    public static bool IsAvailable(this NavMeshPath path, Vector3 targetPosition)
    {
        return path.corners.Length > 0 && path.corners[^1] == targetPosition;
    }
}
