using System;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class CharacterFOV : CharacterAbility
{
    [Space(40f)]
    
    [SerializeField] private float raysPerDeg = 1f;
    [SerializeField] private float verticalRaycastOffset;
    [SerializeField] private LayerMask mask;
    [SerializeField] private Options options;
    
    [Header("Links")]
    [SerializeField] private MeshFilter meshFilter;
    
    [Header("Debug")]
    [SerializeField] private bool drawGizmo;
    [SerializeField] private bool buildMesh = true;

    private Vector3 _lastPosition;
    private Vector3 _lastRotation;
    private ThiefLevel _level;
    private FovMeshBuilder _fovMeshBuilder;
    
    private List<Vector3> _pointsInVision = new List<Vector3>();
    private List<CharacterVisionTarget> _charactersInVision = new List<CharacterVisionTarget>();

    public IReadOnlyCollection<Vector3> PointsInVision => _pointsInVision?.AsReadOnly();
    public IReadOnlyCollection<CharacterVisionTarget> CharactersInVision => _charactersInVision?.AsReadOnly();

    public void Build(Options options)
    {
        this.options = options;
    }

    public override void ProcessAbility()
    {
        base.ProcessAbility();
        
        if (!AbilityAuthorized || !AbilityPermitted)
        {
            meshFilter.gameObject.SetActive(false);
            _pointsInVision.Clear();
            _charactersInVision.Clear();

            return;
        }

        meshFilter.gameObject.SetActive(true);
        TryUpdateVision();
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        
        meshFilter.gameObject.SetActive(false);
    }

    protected override void Initialization()
    {
        base.Initialization();
        
        _level = GetComponentInParent<ThiefLevel>();
        InitializeFovMeshBuilder();
    }

    protected override void OnEnable()
    {
        meshFilter.gameObject.SetActive(true);
    }

    protected override void OnDisable()
    {
        _charactersInVision.Clear();
        meshFilter.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmo)
        {
            return;
        }
        
        foreach (Vector3 point in _pointsInVision)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(point, 0.5f);
        }
    }

    private void InitializeFovMeshBuilder()
    {
        _fovMeshBuilder = new FovMeshBuilder(new FovMeshBuilder.Options
        {
            angle = options.Angle,
            raysPerDeg = raysPerDeg,
            distance = options.DistanceVision,
            meshFilter = meshFilter,
            raycastMask = mask,
            raycastOffset = new Vector3(0, verticalRaycastOffset, 0)
        });
    }

    private void TryUpdateVision()
    {
        _fovMeshBuilder.BuildMesh();
        FindCharactersInVision();
        FindPointsInVision();
    }

    private void FindCharactersInVision()
    {
        _charactersInVision = new List<CharacterVisionTarget>();
        foreach (CharacterVisionTarget target in CharacterVisionTarget.Targets)
        {
            if (InPositionInFov(target.transform.position))
            {
                _charactersInVision.Add(target);
            }
        }
    }
    
    private void FindPointsInVision()
    {
        _pointsInVision = new List<Vector3>();
        foreach (Vector3 point in _level.Points)
        {
            if (InPositionInFov(point))
            {
                _pointsInVision.Add(point);
            }
        }
    }

    private bool InPositionInFov(Vector3 position)
    {
        Vector3 pointDirection = meshFilter.transform.InverseTransformPoint(position);
        float angle = Vector3.SignedAngle(Vector3.forward, pointDirection, Vector3.up);
        
        Vector3 nearestVert = FindNearestVert(angle, _fovMeshBuilder.Verts.ToList());
        bool isAngleOk = Mathf.Abs(_fovMeshBuilder.VertAngles[nearestVert] - angle) <= _fovMeshBuilder.AngleStep;
        bool isDistanceOk = pointDirection.magnitude < nearestVert.magnitude;

        return isAngleOk && isDistanceOk; 
    }
    
    private Vector3 FindNearestVert(float angle, List<Vector3> verts)
    {
        int half = verts.Count / 2;
        
        if (verts.Count == 1)
        {
            return verts[0];
        }
        
        if (VertAngle(verts[half]) < angle)
        {
            return FindNearestVert(angle, verts.GetRange(0, half));
        }
        
        return FindNearestVert(angle, verts.GetRange(half, verts.Count - half));
    }

    private float VertAngle(Vector3 vert)
    {
        return _fovMeshBuilder.VertAngles[vert];        
    }

    [Serializable]
    public class Options
    {
        public float DistanceVision = 5f;
        public float Angle = 90f;
    }
}