using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Dreamteck.Splines;
using Larje.Core.Services;
using MoreMountains.TopDownEngine;
using ProjectConstants;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerSpawner : MonoBehaviour, ILevelEventHandler, ILevelStartHandler, ILevelEndHandler
{
    [SerializeField] private float delay;
    [SerializeField] private GameObject content;
    
    [Header("Animation")]
    [SerializeField] private float spawnAnimDuration;
    [SerializeField] private float spawnAnimDelay;
    [SerializeField] private Ease spawnAnimEase;
    
    [Header("Player")] 
    [SerializeField] private Transform playerRoot;
    [SerializeField] private SplineComputer trajectory;

    [Header("UI")] 
    [SerializeField] private OffscreenMarker markerPrefab;
    [SerializeField] private Image progressUi;
        
    [InjectService] private ILevelManagerService _levelService;

    private bool _levelPlaying;
    private bool _triggerActive;
    private bool _playerTouched;
    private bool _despawning;
    private bool _minProgressAchieved;
    private float _currentTime;
    private OffscreenMarker _marker;
    private SellPoint _sellPoint;
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
            _minProgressAchieved = true;
        }
    }

    private void Start()
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        
        GrabVanMovement();
        GrabPlayer();
        
        _sellPoint = transform.parent.GetComponentInChildren<SellPoint>();
        _marker = Instantiate(markerPrefab).Init(transform, IsMarkerActive);
    }
    
    private void Update()
    {
        CheckIsActive();
        
        if (!_despawning && _playerTouched && _levelPlaying && _triggerActive)
        {
            _currentTime += Time.deltaTime;
            if (_currentTime >= delay)
            {
                Despawn();
                _currentTime = Mathf.Min(_currentTime, delay);
            }
        }
        else
        {
            _currentTime = 0f;
        }

        progressUi.fillAmount = _currentTime / delay;
    }
    
    private void OnDestroy()
    {
        if (_marker != null)
        {
            Destroy(_marker.gameObject);
        }
    }

    private void GrabVanMovement()
    {
        _vanMovement = GetComponentInParent<VanMovement>();
        _vanMovement.EventStartAnimationComplete += Spawn;
    }

    private void GrabPlayer()
    {
        _player = playerRoot.GetComponentInChildren<CharacterSpawn>();
        _player.SetSpawningState(playerRoot, CharacterSpawn.SpawningDirection.Out);
        trajectory.RebuildImmediate();
        playerRoot.position = EvaluateTrajectory(1f);
    }

    private void Spawn()
    {
        this.DOKill();
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(spawnAnimDelay);
        seq.Append(DOTween.To(() => 1f, (x) => playerRoot.position = EvaluateTrajectory(x), 0f, spawnAnimDuration)
            .SetEase(spawnAnimEase)
            .SetTarget(this)
            .OnComplete(() =>_player.SetNormalState()));
    }

    private void Despawn()
    {
        _despawning = true;
        _player.SetSpawningState(playerRoot, CharacterSpawn.SpawningDirection.In);
        
        this.DOKill();
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(spawnAnimDelay);
        seq.Append(DOTween.To(() => 0f, (x) => playerRoot.position = EvaluateTrajectory(x), 1f, spawnAnimDuration)
            .SetEase(spawnAnimEase)
            .SetTarget(this)
            .OnComplete(Win));
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

    private void CheckIsActive()
    {
        _triggerActive = _minProgressAchieved && !_sellPoint.TriggerActive;
        
        content.SetActive(_triggerActive);
    }

    private void Win()
    {
        _levelService.TryStopCurrentLevel(new LevelProcessor.StopData(LevelStopType.Win));
    }

    private Vector3 EvaluateTrajectory(float value)
    {
        return trajectory.EvaluatePosition(value);
    }

    private bool IsMarkerActive()
    {
        return _minProgressAchieved && !_sellPoint.TriggerActive && _levelPlaying;
    }
}
