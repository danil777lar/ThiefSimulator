using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhysicsUtility
{
    public static Dictionary<T, Collider> FindObjectsInRange<T>(Vector3 selfPosition, float radius, LayerMask targetMask, LayerMask obstaclesMask) where T : MonoBehaviour
    {
        Dictionary<T, Collider> result = new Dictionary<T, Collider>();
        
        Collider[] colliders = Physics.OverlapSphere(selfPosition, radius, targetMask);
        foreach (Collider collider in colliders)
        {
            T target = collider.GetComponentInParent<T>();
            if (target != null)
            {
                Vector3 from = selfPosition;
                from.y = collider.transform.position.y;

                Vector3 direction = collider.transform.position - from;
                from += Vector3.up * 0.1f;
                if (!Physics.Raycast(from, direction, out RaycastHit hit, direction.magnitude, obstaclesMask) || hit.collider == collider)
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
