using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class AttackMarker : MonoBehaviour
{
    private MeshFilter _meshFilter;

    public void Remove()
    {
        Destroy(gameObject);
    }
    
    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        
        FovMeshBuilder.Input input = new FovMeshBuilder.Input
        {
            angle = 45,
            raysPerAngle = 1f,
            distance = 3f,
            meshFilter = _meshFilter
        };
        
        FovMeshBuilder.Output output = FovMeshBuilder.BuildMesh(input);
    }
}
