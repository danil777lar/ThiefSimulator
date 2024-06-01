using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class AttackMarker : MonoBehaviour
{
    private MeshFilter _meshFilter;

    public void Init(float distance, float angle, Vector3 direction)
    {
        _meshFilter = GetComponent<MeshFilter>();
        
        FovMeshBuilder.Input input = new FovMeshBuilder.Input
        {
            angle = angle,
            directionRotate = 180f + Quaternion.LookRotation(direction).eulerAngles.y,
            raysPerAngle = 1f,
            distance = distance,
            meshFilter = _meshFilter
        };
        
        FovMeshBuilder.Output output = FovMeshBuilder.BuildMesh(input);   
    }
    
    public void Remove()
    {
        Destroy(gameObject);
    }
}
