using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public static class PhysicsUtility
{
    public static Dictionary<T, Collider> FindObjectsInRange<T>(Vector3 selfPosition, float radius, LayerMask targetMask, LayerMask obstaclesMask) where T : MonoBehaviour
    {
        Dictionary<T, Collider> result = new Dictionary<T, Collider>();
        
        Collider[] colliders = Physics.OverlapSphere(selfPosition, radius, targetMask);
        foreach (Collider collider in colliders)
        {
            if (collider.attachedRigidbody != null 
                && collider.attachedRigidbody.TryGetComponent(out T target))
            {
                Vector3 from = selfPosition.MMSetY(collider.transform.position.y);
                Vector3 direction = collider.transform.position - from;
                from += Vector3.up * 0.1f;
                if (!Physics.Raycast(from, direction, out RaycastHit hit,
                        direction.magnitude, obstaclesMask) || hit.collider == collider)
                {
                    if (!result.ContainsKey(target))
                    {
                        result.Add(target, collider);   
                    }
                }
            }
        }

        return result;
    }
}
