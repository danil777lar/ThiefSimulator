using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Larje.Core.Services;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterSpawn : CharacterAbility
{
    [SerializeField] private float fitAnimationDuration = 0.5f;
    
    private bool _isSpawning;
    private SpawningDirection _direction;
    private TopDownController _topDownController;
    private CharacterController _characterController;
    private LevelProcessor _level;
    
    protected const string _spawnOutAnimationParameterName = "SpawnOut";
    protected const string _spawnInAnimationParameterName = "SpawnIn";
    protected int _spawnOutAnimationParameter;
    protected int _spawnInAnimationParameter;

    public void SetSpawningState(Transform parent, SpawningDirection direction)
    {
        _isSpawning = true;
        _direction = direction;
        
        _topDownController.GravityActive = false;
        _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Frozen);
        _characterController.enabled = false;
        transform.SetParent(parent);

        transform.DOLocalMove(Vector3.zero, fitAnimationDuration);
        transform.DOLocalRotate(Vector3.up * (direction == SpawningDirection.Out ? 180f : 0f), fitAnimationDuration);
        _character.CharacterModel.transform.DOLocalRotate(Vector3.zero, fitAnimationDuration);
    }

    public void SetNormalState()
    {
        _isSpawning = false;
        
        _topDownController.GravityActive = true;
        _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
        _characterController.enabled = true;
        transform.SetParent(_level.transform);
    }
    
    public override void UpdateAnimator()
    {
        base.UpdateAnimator();

        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _spawnOutAnimationParameter, 
            _isSpawning && _direction == SpawningDirection.Out,
            _character._animatorParameters, _character.RunAnimatorSanityChecks);
        
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _spawnInAnimationParameter, 
            _isSpawning && _direction == SpawningDirection.In,
            _character._animatorParameters, _character.RunAnimatorSanityChecks);
    }

    protected override void Initialization()
    {
        base.Initialization();
        
        _level = GetComponentInParent<LevelProcessor>();
        _characterController = _character.GetComponent<CharacterController>();
        _topDownController = _character.GetComponent<TopDownController>();
    }
    
    protected override void InitializeAnimatorParameters()
    {
        base.InitializeAnimatorParameters();

        RegisterAnimatorParameter(_spawnOutAnimationParameterName, AnimatorControllerParameterType.Bool,
            out _spawnOutAnimationParameter);
        
        RegisterAnimatorParameter(_spawnInAnimationParameterName, AnimatorControllerParameterType.Bool,
            out _spawnInAnimationParameter);
    }

    public enum SpawningDirection
    {
        Out,
        In
    }
}
