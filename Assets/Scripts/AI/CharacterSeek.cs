using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.AI;

public class CharacterSeek : MonoBehaviour
{
    [SerializeField] private float seekDistance;

    private CharacterFOV _fov;
    private ThiefLevel _level;
    private Vector3 _lastPlayerPoint;
    private List<Vector3> _seekPoints;

    public bool PlayerInVision { get; private set; }
    public bool IsSeek { get; private set; }
    public Vector3 SeekPoint { get; private set; }
    public IReadOnlyCollection<Vector3> SeekPoints => _seekPoints;

    private void Start()
    {
        _fov = GetComponent<CharacterFOV>();
        _level = GetComponentInParent<ThiefLevel>();
    }

    private void Update()
    {
        TrySeePlayer();
        LookToPoints();
    }

    private void OnDrawGizmos()
    {
        if (_fov != null && _fov.PointsInVision != null)
        {
            Gizmos.color = Color.blue;
            foreach (Vector3 point in _fov.PointsInVision)
            {
                Gizmos.DrawSphere(point, 0.35f);
            }
        }

        if (SeekPoints != null)
        {
            Gizmos.color = Color.red;
            foreach (Vector3 point in SeekPoints)
            {
                Gizmos.DrawSphere(point, 0.4f);
            }
        }
    }

    private void TrySeePlayer()
    {
        Character player = _fov.CharactersInVision.ToList()
            .Find(x => x.CharacterType == Character.CharacterTypes.Player);

        bool playerInVision = player != null;
        if (playerInVision)
        {
            _lastPlayerPoint = player.transform.position;
            StopSeek();
        }
        else if (PlayerInVision)
        {
            StartSeek(_lastPlayerPoint);
        }

        PlayerInVision = playerInVision;
    }

    private void LookToPoints()
    {
        if (IsSeek && _seekPoints != null)
        {
            foreach (Vector3 point in _seekPoints.ToArray())
            {
                if (_fov.PointsInVision.Contains(point))
                {
                    _seekPoints.Remove(point);
                }
            }

            if (_seekPoints.Count == 0)
            {
                StopSeek();
            }
        }
    }

    private void StartSeek(Vector3 position)
    {
        IsSeek = true;
        SeekPoint = position;

        _seekPoints = _level.Points.ToList().FindAll(x =>
        {
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(SeekPoint, x, NavMesh.AllAreas, path))
            {
                return path.IsAvailable(x) && path.GetLength() <= seekDistance;
            }
            return false;
        });
    }
    
    private void StopSeek()
    {
        IsSeek = false;
        _seekPoints = null;
    }
}