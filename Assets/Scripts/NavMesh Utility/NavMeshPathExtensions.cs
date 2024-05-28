using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
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

    public static List<Vector3> GetShiftedCorners(this NavMeshPath path, float distance)
    {
        List<Vector3> shiftedCorners = new List<Vector3>(path.corners);
        
        if (shiftedCorners.Count > 1)
        {
            for (int i = 1; i < shiftedCorners.Count - 1; i++)
            {
                Vector3 normal = path.corners[i + 1] - path.corners[i - 1];
                Vector3 point = path.corners[i] - path.corners[i - 1];
                Vector3 projection = Vector3.Project(point, normal) + path.corners[i - 1];
                Vector3 direction = (path.corners[i] - projection).normalized;
                
                Debug.DrawRay(projection, Vector3.up * 2f, Color.blue);
                Debug.DrawRay(projection + Vector3.up * 0.1f, direction * distance, Color.blue);
                
                shiftedCorners[i] += direction * distance;
            }
        }

        return shiftedCorners;
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
