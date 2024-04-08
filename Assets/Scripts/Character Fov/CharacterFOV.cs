using System;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterFOV : CharacterAbility
{
    [SerializeField] private bool drawGizmo;
    [SerializeField] private bool buildMesh = true;
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
        if (AbilityAuthorized && AbilityPermitted)
        {
            UpdateVision();
        }
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
        if (options == null)
        {
            return;
        }

        List<Vector3> rays = new List<Vector3>(); 
        _charactersInVision.Clear();

        Vector3 scale = Vector3.one * meshFilter.transform.parent.InverseTransformVector(Vector3.right).magnitude;
        meshFilter.transform.localScale = scale;

        int rayCount = Mathf.RoundToInt(options.Angle);
        float angle = options.Angle * 0.5f;
        float angleIncrease = options.Angle / rayCount;
        Vector3[] verticles = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[verticles.Length];
        int[] triangles = new int[rayCount * 3];

        verticles[0] = Vector3.zero;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 direction = Vector3.zero;
            direction.x = Mathf.Sin(Mathf.Deg2Rad * angle);
            direction.z = Mathf.Cos(Mathf.Deg2Rad * angle);
            direction.Normalize();

            Vector3 localDirection = meshFilter.transform.TransformDirection(direction);
            Vector3 vertex = Vector3.zero;

            if (Physics.Raycast(meshFilter.transform.position, localDirection, out RaycastHit hit, options.DistanceVision, mask))
            {
                vertex = meshFilter.transform.InverseTransformPoint(hit.point);

                CharacterVisionTarget character = CharacterVisionTarget.Targets.ToList()
                    .Find(x => x.gameObject == hit.collider.gameObject); 
                if (character != null)
                {
                    _charactersInVision.Add(character);
                }
            }
            else
            {
                vertex = direction * options.DistanceVision;
            }
            
            rays.Add(vertex);

            if (drawGizmo)
            {
                Debug.DrawLine(meshFilter.transform.position, meshFilter.transform.TransformPoint(vertex),
                    Color.red);
            }

            verticles[vertexIndex] = vertex;
            if (i > 0)
            {
                triangles[triangleIndex + 0] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = 0;
                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        if (buildMesh)
        {
            if (meshFilter.mesh == null)
            {
                meshFilter.mesh = new Mesh
                {
                    name = "FOV"
                };
            }

            meshFilter.mesh.vertices = verticles;
            meshFilter.mesh.uv = uv;
            meshFilter.mesh.triangles = triangles;
        }
        
        FindPointsInVision(rays, angleIncrease);
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