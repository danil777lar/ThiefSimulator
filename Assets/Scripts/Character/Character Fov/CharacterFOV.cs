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
    private FovMeshBuilder _fovMeshBuilder;
    
    private List<CharacterVisionTarget> _charactersInVision = new List<CharacterVisionTarget>();
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
            _charactersInVision.Clear();

            return;
        }

        meshFilter.gameObject.SetActive(true);
        TryUpdateVision();
    }
    
    public bool IsPointInVision(Vector3 point)
    {
        return _fovMeshBuilder.InPositionInFov(point);
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        
        meshFilter.gameObject.SetActive(false);
    }

    protected override void Initialization()
    {
        base.Initialization();
        
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
    }

    private void FindCharactersInVision()
    {
        _charactersInVision = new List<CharacterVisionTarget>();
        foreach (CharacterVisionTarget target in CharacterVisionTarget.Targets)
        {
            if (_fovMeshBuilder.InPositionInFov(target.transform.position))
            {
                _charactersInVision.Add(target);
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