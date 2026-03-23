using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Larje.Character;
using Larje.Core.Services;
using UnityEngine;

public class CharacterSpawn : CharacterAbility
{
    [SerializeField] private float fitAnimationDuration = 0.5f;
    
    private bool _isSpawning;
    private SpawningDirection _direction;
    private LevelProcessor _level;

    public void SetSpawningState(Transform parent, SpawningDirection direction)
    {
        _isSpawning = true;

        _direction = direction;
        character.transform.SetParent(parent);

        character.transform.DOLocalMove(Vector3.zero, fitAnimationDuration);
        character.transform.DOLocalRotate(Vector3.up * (direction == SpawningDirection.Out ? 180f : 0f), fitAnimationDuration);
        // _character.CharacterModel.transform.DOLocalRotate(Vector3.zero, fitAnimationDuration);
    }

    public void SetNormalState()
    {
        _isSpawning = false;
        character.transform.SetParent(_level.transform);
    }

    protected override void OnInitialized()
    {
        character.IsActiveComposite.AddValue(() => !_isSpawning);
        _level = GetComponentInParent<LevelProcessor>();
    }

    public enum SpawningDirection
    {
        Out,
        In
    }
}
