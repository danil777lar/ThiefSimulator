using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterSeek : MonoBehaviour
{
    private CharacterFOV _fov;

    public bool PlayerInVision { get; private set; }

    private void Start()
    {
        _fov = GetComponent<CharacterFOV>();
    }

    private void Update()
    {
        PlayerInVision = _fov.CharactersInVision.ToList()
            .Find(x => x.CharacterType == Character.CharacterTypes.Player);
    }
}