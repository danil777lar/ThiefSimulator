using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterSeek : MonoBehaviour
{
    private CharacterFOV _fov;

    public bool PlayerInVision { get; private set; }
    public bool IsSeek { get; private set; }
    public Vector3 SeekPoint { get; private set; }

    private void Start()
    {
        _fov = GetComponent<CharacterFOV>();
    }

    private void Update()
    {
        Character player = _fov.CharactersInVision.ToList()
            .Find(x => x.CharacterType == Character.CharacterTypes.Player);
        PlayerInVision = player != null;
        if (PlayerInVision)
        {
            IsSeek = true;
            SeekPoint = player.transform.position;
        }
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
    }
}