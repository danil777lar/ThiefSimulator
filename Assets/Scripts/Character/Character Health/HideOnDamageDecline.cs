using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.AI;

public class HideOnDamageDecline : MonoBehaviour
{
    [SerializeField] private float distance;
    [Header("Effects")] 
    [SerializeField] private ParticleSystem partsOnHide;
    
    private void Start()
    {
        CharacterHealth health = GetComponentInParent<CharacterHealth>();
        if (health)
        {
            health.EventDamageDeclined += Hide;
        }
    }

    private void Hide()
    {
        Vector3 newPosition = FindPoint();
        Instantiate(partsOnHide, transform.position, Quaternion.identity);
        Instantiate(partsOnHide, newPosition, Quaternion.identity);
        transform.position = newPosition;
    }

    private Vector3 FindPoint()
    {
        float angleStep = 10f;
        Vector3 farestPoint = transform.position;
        
        for (float angle = 0; angle < 360; angle += angleStep)
        {
            Vector3 direction = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, Mathf.Cos(angle * Mathf.Deg2Rad));
            Vector3 from = transform.position;
            Vector3 to = from + direction.normalized * distance;
            if (NavMesh.Raycast(from, to, out NavMeshHit hit, NavMesh.AllAreas))
            {
                if (Vector3.Distance(from, hit.position) > Vector3.Distance(from, farestPoint))
                {
                    farestPoint = hit.position;
                }
            }
            else
            {
                farestPoint = to;
            }
        }

        return farestPoint;
    }
}
