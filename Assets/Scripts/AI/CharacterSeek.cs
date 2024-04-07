using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.TopDownEngine;
using ToonyColorsPro.Legacy;
using UnityEngine;
using UnityEngine.AI;

public class CharacterSeek : CharacterAbility
{
    [SerializeField] private CharacterSeekConfig config;

    private CharacterFOV _fov;
    private ThiefLevel _level;
    private Vector3 _lastPlayerPoint;
    private Character _player;
    private List<Vector3> _seekPoints;
    private CharacterController _characterController;

    public bool PlayerInVision { get; private set; }
    public bool IsAttack { get; private set; }
    public bool IsSeek { get; private set; }
    public Vector3 SeekPoint { get; private set; }
    public IReadOnlyCollection<Vector3> SeekPoints => _seekPoints;

    private void Start()
    {
        _fov = GetComponent<CharacterFOV>();
        _level = GetComponentInParent<ThiefLevel>();
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        TrySeePlayer();
        LookToPoints();
        TrySendDamage();
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
        _player = _fov.CharactersInVision.ToList()
            .Find(x => x.CharacterType == Character.CharacterTypes.Player);

        bool playerInVision = _player != null;
        if (playerInVision)
        {
            _lastPlayerPoint = _player.transform.position;
            StopSeek();
        }
        else if (PlayerInVision)
        {
            StartSeek(_lastPlayerPoint);
        }

        PlayerInVision = playerInVision;
    }

    private void TrySendDamage()
    {
        if (PlayerInVision && _player != null)
        {
            Vector3 directionToPlayer = _player.transform.position - transform.position;
            if (directionToPlayer.magnitude <= config.AttackDistance)
            {
                Ray ray = new Ray(_characterController.transform.position + (Vector3.up * (_characterController.height * 0.5f)), 
                    _character.CharacterModel.transform.forward);
                float radius = _characterController.radius;
                LayerMask mask = LayerMask.GetMask(LayerMask.LayerToName(_player.gameObject.layer));
                if (Physics.SphereCast(ray, radius, config.AttackDistance, mask))
                {
                    _character.CharacterAnimator.SetTrigger("Ram");
                    _player.CharacterHealth.Damage(1, gameObject, 0f, 0f, 
                        directionToPlayer.normalized);
                    
                    StartCoroutine(AttackCooldown());
                }
            }      
        }
    }
    
    private IEnumerator AttackCooldown()
    {
        IsAttack = true;
        yield return new WaitForSeconds(config.AttackCooldown);
        IsAttack = false;
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
                return path.IsAvailable(x) && path.GetLength() <= config.SeekDistance;
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