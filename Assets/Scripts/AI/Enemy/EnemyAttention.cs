using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class EnemyAttention : CharacterAbility
{
    [SerializeField] private EnemyAttentionConfig config;
    [SerializeField] private List<StateBrain> brains;

    private float _lastDecreaseTime;
    private float _attentionDecreaseDelay;
    private Vector3 _lastPlayerPoint;
    
    private CharacterFOV _fov;
    private SoundReceiver _soundReceiver;
    private CharacterVisionTarget _player;
    private CharacterController _characterController;

    public bool IsAttack { get; private set; }
    public float AttentionLevel { get; private set; }
    public float MaxSuspicion => config.MaxSuspicion;
    public float MaxAggression => config.MaxAggression;
    public Vector3 LastAttentionPoint { get; private set; }
    public AttentionState CurrentState { get; private set; }

    public override void ProcessAbility()
    {
        base.ProcessAbility();
        
        TrySeePlayer();
        DecreaseAttention();
        TrySendDamage();
    }

    protected override void Initialization()
    {
        base.Initialization();

        _lastDecreaseTime = Time.time;
        _fov = GetComponent<CharacterFOV>();
        _characterController = GetComponent<CharacterController>();
        _soundReceiver = GetComponent<SoundReceiver>();
        
        _soundReceiver.EventSoundReceived += OnSoundReceived;
        SetState(AttentionState.Idle, true);
    }

    private void TrySeePlayer()
    {
        CharacterVisionTarget player = _fov.CharactersInVision.ToList()
            .Find(x => x.Character.CharacterType == Character.CharacterTypes.Player);
        
        if (player)
        {
            _player = player;
            float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
            float playerVisibility = Mathf.Clamp01(1f - (distanceToPlayer / config.VisionDistance)) * _player.Visibility;
            AddAttention(playerVisibility * config.VisionSensitivity * Time.deltaTime, player.transform.position); 
        }
    }
    
    private void AddAttention(float attention, Vector3 position, bool onlySuspicion = false)
    {
        if (onlySuspicion)
        {
            attention = Mathf.Min(attention, MaxSuspicion - AttentionLevel);
            attention = Mathf.Max(attention, 0);
        }

        LastAttentionPoint = position;
        if (attention > 0f)
        {
            AttentionLevel = Mathf.Clamp(AttentionLevel + attention, 0f, MaxSuspicion + MaxAggression);
            
            if (AttentionLevel >= MaxAggression + MaxSuspicion)
            {
                SetState(AttentionState.Aggressive);
            }
            else if (AttentionLevel >= MaxSuspicion)
            {
                SetState(AttentionState.Suspicious);
            }

            _attentionDecreaseDelay = AttentionLevel < MaxSuspicion
                ? config.SuspicionDecreaseDelay
                : config.AggressionDecreaseDelay;
        }
    }

    private void DecreaseAttention()
    {
        float deltaTime = Time.time - _lastDecreaseTime;
        _lastDecreaseTime = Time.time;
        
        if (_attentionDecreaseDelay > 0f)
        {
            _attentionDecreaseDelay -= deltaTime;
        }
        else if (AttentionLevel > 0f)
        {
            AttentionLevel -= deltaTime * (CurrentState == AttentionState.Suspicious
                ? config.SuspicionDecreaseSpeed
                : config.AggressionDecreaseSpeed);

            if (AttentionLevel <= MaxSuspicion && CurrentState == AttentionState.Aggressive)
            {
                SetState(AttentionState.Suspicious);
            }
        }
        else
        {
            SetState(AttentionState.Idle);
        }
    }

    private void TrySendDamage()
    {
        if (_player != null)
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
                    _player.Character.CharacterHealth.Damage(1, gameObject, 0f, 0f, 
                        directionToPlayer.normalized);
                    
                    StartCoroutine(AttackCooldown());
                }
            }      
        }
    }
    
    private void OnSoundReceived(float amplitude, Vector3 position)
    {
        AddAttention(amplitude * config.HearingSensitivity, position, true);   
    }

    private void SetState(AttentionState state, bool forceSwap = false)
    {
        AIBrain brain = brains.Find(x => x.AttentionState == state).Brain;

        if (forceSwap || state != CurrentState)
        {
            _character.CharacterBrain = brain;
            brain.gameObject.SetActive(true);
            brain.enabled = true;
            brain.Owner = _character.gameObject;
            brain.ResetBrain();

            brains.ForEach(x =>
            {
                if (x.Brain != brain)
                {
                    x.Brain.gameObject.SetActive(false);
                    x.Brain.enabled = false;
                }
            });
        }

        CurrentState = state;
    }

    private IEnumerator AttackCooldown()
    {
        IsAttack = true;
        yield return new WaitForSeconds(config.AttackCooldown);
        IsAttack = false;
    }
    
    public enum AttentionState
    {
        Idle,
        Suspicious,
        Aggressive
    }

    [Serializable]
    private class StateBrain
    {
        [field: SerializeField] public AttentionState AttentionState { get; private set; }
        [field: SerializeField] public AIBrain Brain { get; private set; }
    }
}