using System;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterFOV : MonoBehaviour
{
    [HideInInspector] public float visionDistanceModifier = 1f;

    [SerializeField] private bool drawGizmo;
    [SerializeField] private bool buildMesh = true;
    [SerializeField] private LayerMask mask;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private Options options;
    
    private List<Character> _charactersInVision = new List<Character>();
    private List<Character> _charactersInShoot = new List<Character>();

    public Character[] CharactersInVision => _charactersInVision.ToArray();
    public Character[] CharactersInShoot => _charactersInShoot.ToArray();

    public void Build(Options options)
    {
        this.options = options;
    }

    public float GetDistance()
    {
        return options.DistanceVision * visionDistanceModifier;
    }

    private void Awake()
    {
        visionDistanceModifier = 1f;
    }

    private void OnEnable()
    {
        meshFilter.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        _charactersInVision.Clear();
        _charactersInShoot.Clear();
        meshFilter.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateVision();
    }

    private void UpdateVision()
    {
        if (options == null)
        {
            return;
        }

        _charactersInVision.Clear();
        _charactersInShoot.Clear();

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

            if (Physics.Raycast(meshFilter.transform.position, localDirection, out RaycastHit hit, GetDistance(), mask))
            {
                vertex = meshFilter.transform.InverseTransformPoint(hit.point);
            }
            else
            {
                vertex = direction * GetDistance();
            }

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
    }

    [Serializable]
    public class Options
    {
        public float DistanceVision = 5f;
        public float Angle = 90f;
    }
}