using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyPatrolProcessor
{
    public bool TryGetPosition(out Vector3 position);
    public bool TryGetLookPosition(out Vector3 lookPosition);
}
