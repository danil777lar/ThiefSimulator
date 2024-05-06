using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Larje.Core.Services;
using MoreMountains.TopDownEngine;
using ProjectConstants;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerSpawner : MonoBehaviour, ILevelEventHandler, ILevelStartHandler, ILevelEndHandler
{
    [SerializeField] private float delay;
    
    [Header("Player")] 
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform pointIn;
    
    [Header("UI")]
    [SerializeField] private GameObject uiRoot;
    [SerializeField] private Image progressUi;
        
    [InjectService] private ILevelManagerService _levelService;

    private bool _levelPlaying;
    private bool _triggerActive;
    private bool _playerTouched;
    private float _currentTime;
    private CharacterSpawn _player;
    private VanMovement _vanMovement;
    
    public void OnLevelStarted(LevelProcessor.StartData data)
    {
        _levelPlaying = true;
    }

    public void OnLevelEnded(LevelProcessor.StopData data)
    {
        _levelPlaying = false;
    }
    
    public void OnLevelEvent(LevelEvent levelEvent)
    {
        if (levelEvent is LevelEventProgressComplete { Type: LevelEventProgressComplete.ProgressType.Min })
        {
            SetActiveTrigger(true);
        }
    }

    private void Start()
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        SetActiveTrigger(false);
        
        GrabVanMovement();
        GrabPlayer();
    }
    
    private void Update()
    {
        if (_playerTouched && _levelPlaying && _triggerActive)
        {
            _currentTime += Time.deltaTime;
            if (_currentTime >= delay)
            {
                Win();
                _currentTime = Mathf.Min(_currentTime, delay);
            }
        }
        else
        {
            _currentTime = 0f;
        }

        progressUi.fillAmount = _currentTime / delay;
    }

    private void GrabVanMovement()
    {
        _vanMovement = GetComponentInParent<VanMovement>();
        _vanMovement.EventStartAnimationComplete += Spawn;
    }

    private void GrabPlayer()
    {
        _player = playerRoot.GetComponentInChildren<CharacterSpawn>();
        _player.SetSpawningState(playerRoot);
        playerRoot.position = pointIn.position;
    }

    private void Spawn()
    {
        playerRoot.DOLocalMove(Vector3.up * pointIn.localPosition.y, 0.5f)
            .OnComplete(() =>
            {
                playerRoot.DOLocalMove(Vector3.zero, 0.25f)
                    .OnComplete(() => _player.SetNormalState());
            });
    }

    private void Despawn()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_playerTouched && other.gameObject == _player.gameObject)
        {
            _playerTouched = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_playerTouched && other.gameObject == _player.gameObject)
        {
            _playerTouched = false;
        }
    }

    private void SetActiveTrigger(bool arg)
    {
        uiRoot.SetActive(arg);
        _triggerActive = arg;
    }

    private void Win()
    {
        _levelService.TryStopCurrentLevel(new LevelProcessor.StopData(LevelStopType.Win));
    }
}
