using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterSpawn : CharacterAbility
{
    private TopDownController _topDownController;
    private CharacterController _characterController;
    private LevelProcessor _level;

    public void SetSpawningState(Transform parent)
    {
        _topDownController.GravityActive = false;
        _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Frozen);
        _characterController.enabled = false;
        transform.SetParent(parent);
    }

    public void SetNormalState()
    {
        _topDownController.GravityActive = true;
        _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
        _characterController.enabled = true;
        transform.SetParent(_level.transform);
    }

    protected override void Initialization()
    {
        _level = GetComponentInParent<LevelProcessor>();
        _characterController = _character.GetComponent<CharacterController>();
        _topDownController = _character.GetComponent<TopDownController>();
    }
}
