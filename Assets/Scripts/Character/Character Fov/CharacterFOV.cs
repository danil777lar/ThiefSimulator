using System;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class CharacterFOV : CharacterAbility
{
    [SerializeField] private bool drawGizmo;
    [SerializeField] private bool buildMesh = true;
    [SerializeField] private float verticalRaycastOffset;
    [SerializeField] private LayerMask mask;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private Options options;

    private ThiefLevel _level;
    private List<Vector3> _pointsInVision = new List<Vector3>();
    private List<CharacterVisionTarget> _charactersInVision = new List<CharacterVisionTarget>();

    public IReadOnlyCollection<Vector3> PointsInVision => _pointsInVision.AsReadOnly();
    public IReadOnlyCollection<CharacterVisionTarget> CharactersInVision => _charactersInVision.AsReadOnly();

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
        UpdateVision();
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

    private void UpdateVision()
    {
        FovMeshBuilder.Output fovOutput = FovMeshBuilder.BuildMesh(GetFovMeshInput());
        
        FindCharactersInVision(fovOutput.hits);
        FindPointsInVision(fovOutput.verts, fovOutput.angleIncrease);
    }

    private FovMeshBuilder.Input GetFovMeshInput()
    {
        return new FovMeshBuilder.Input
        {
            angle = options.Angle,
            raysPerAngle = 1f,
            distance = options.DistanceVision,
            meshFilter = meshFilter,
            raycastMask = mask,
            raycastOffset = new Vector3(0, verticalRaycastOffset, 0)
        };
    }

    private void FindCharactersInVision(List<RaycastHit> hits)
    {
        _charactersInVision.Clear();
        foreach (RaycastHit hit in hits)
        {
            CharacterVisionTarget character = CharacterVisionTarget.Targets.ToList()
                .Find(x => x.Character.gameObject == hit.collider.gameObject);
            if (character != null)
            {
                _charactersInVision.Add(character);
            }
        }
    }
    
    private void FindPointsInVision(List<Vector3> rays, float angle)
    {
        _pointsInVision.Clear();
        foreach (Vector3 point in _level.Points)
        {
            Vector3 pointDirection = meshFilter.transform.InverseTransformPoint(point);
            List<Vector3> nearestRays = rays.FindAll(x => 
                Vector3.Angle(x.normalized.XZ(), pointDirection.normalized.XZ()) <= angle);
            if (nearestRays.Count > 0 && pointDirection.magnitude < nearestRays.First().magnitude)
            {
                _pointsInVision.Add(point);
            }
        }
    }

    [Serializable]
    public class Options
    {
        public float DistanceVision = 5f;
        public float Angle = 90f;
    }
}