using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class AttackMarker : MonoBehaviour
{
    [SerializeField] private LayerMask mask;

    private float _distance;
    private float _angle;
    private Vector3 _direction;
    private MeshFilter _meshFilter;

    public void Init(float distance, float angle, Vector3 direction)
    {
        _distance = distance;
        _angle = angle;
        _direction = direction;
        
        _meshFilter = GetComponent<MeshFilter>();
     
        transform.localPosition = transform.localPosition.MMSetY(0.05f);
    }
    
    public void Remove()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        FovMeshBuilder.Input input = new FovMeshBuilder.Input
        {
            angle = _angle,
            directionRotate = 180f + Quaternion.LookRotation(_direction).eulerAngles.y,
            raysPerAngle = 1f,
            distance = _distance,
            meshFilter = _meshFilter,
            raycastMask = mask,
        };
        FovMeshBuilder.Output output = FovMeshBuilder.BuildMesh(input);
    }
}
