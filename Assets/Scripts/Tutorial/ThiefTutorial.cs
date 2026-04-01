using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Larje.Character;
using Larje.Core;
using Larje.Core.Services;
using TMPro;
using UnityEngine;

public class ThiefTutorial : MonoBehaviour
{
    [SerializeField] private OffscreenMarker markerPrefab;

    [Header("Move Step")] 
    [SerializeField] private float moveStepDistance = 5f;
    [SerializeField] private Transform moveTargetPoint;
    
    [Header("Loot Step")] 
    [SerializeField] private GameObject doorTrigger;
    [SerializeField] private GameObject doorBlocker;
    [SerializeField] private List<Sellable> loot;

    [InjectService] private GameEventService _gameEventService;
    [InjectService] private IAnalyticsService _analyticsService;
    [InjectService] private IPlayerProviderService _playerProviderService;
    [InjectService] private IGameStateService _gameStateService;

    private bool _moveStep;
    private bool _lootStep;

    private bool _showMarker;
    
    private Transform _markerTarget;
    private Transform _markerCenter;

    private Character _player;
    private CharacterCarry3D _playerCarry;

    private OffscreenMarker _markerInstance;
    private TextMeshProUGUI _tutorialText;
    
    private void Start()
    {
        DIContainer.InjectTo(this);

        _gameEventService.Subscribe<LevelEventProgressComplete>(OnLevelProgressComplete);
        
        _tutorialText = GetComponentInChildren<TextMeshProUGUI>();

        if (_playerProviderService.TryGetPlayer(out Character player))
        {
            _player = player;
            _playerCarry = player.GetComponentInChildren<CharacterCarry3D>();
            
            _markerCenter = player.transform;
            _markerTarget = new GameObject("Tutorial Marker Target").transform;
            _markerTarget.SetParent(transform);
            
            _markerInstance = Instantiate(markerPrefab).Init(_markerTarget, _markerCenter, IsMarkerActive);
            
            StartMoveStep();
        }
        else
        {
            UnityEngine.Debug.LogError("Player not found", this);
        }
    }

    private void OnDisable()
    {
        if (_gameStateService != null)
        {
            _gameEventService.Unsubscribe<LevelEventProgressComplete>(OnLevelProgressComplete);
        }

        if (_markerInstance != null)
        {
            Destroy(_markerInstance.gameObject);
        }
    }

    private void Update()
    {
        Color targetColor = _gameStateService.CurrentState == GameStates.Playing ? Color.white : Color.clear;
        _tutorialText.color = Color.Lerp(_tutorialText.color, targetColor, Time.deltaTime * 5f);

        UpdateMoveStep();
        UpdateGrabLootStep();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        if (moveTargetPoint != null)
        {
            Gizmos.DrawWireSphere(moveTargetPoint.position, moveStepDistance);
        }
    }

    private void OnLevelProgressComplete(LevelEventProgressComplete evnt)
    {
        StartGoAwayStep();
    }

    private bool IsMarkerActive()
    {
        return _showMarker && _gameStateService.CurrentState == GameStates.Playing;
    }

    private void StartMoveStep()
    {
        _moveStep = true;

        _showMarker = true;
        SetNewTitle("Move your finger to move");
        _markerTarget.position = moveTargetPoint.position;
    }

    private void UpdateMoveStep()
    {
        if (!_moveStep) return;

        if (Vector3.Distance(_player.transform.position, moveTargetPoint.position) < moveStepDistance)
        {
            _moveStep = false;
            _analyticsService.SendEvent("Tutorial_Move_Complete");
            StartGrabLootStep();
        }
    }

    private void StartGrabLootStep()
    {
        _lootStep = true;

        _showMarker = true;
        SetNewTitle("Approach the loot to pick it up");

        doorTrigger.SetActive(false);
        doorBlocker.SetActive(true);
    }

    private void UpdateGrabLootStep()
    {
        if (!_lootStep) return;

        List<Sellable> lootToPickup = loot.FindAll(s => !_playerCarry.CurrentCarryables.ToList().Find(c => c.gameObject == s.gameObject));

        if (lootToPickup.Count == 0)
        {
            doorTrigger.SetActive(true);
            doorBlocker.SetActive(false);

            _lootStep = false;
            _analyticsService.SendEvent("Tutorial_Grab_Complete");
            StartSellLootStep();
        }
        else
        {
            _markerTarget.position = lootToPickup[0].transform.position;
        }
    }

    private void StartSellLootStep()
    {
        _showMarker = false;
        SetNewTitle("Go to the exit to sell your loot");
    }

    private void StartGoAwayStep()
    {
        _analyticsService.SendEvent("Tutorial_Sell_Complete");

        _showMarker = false;
        SetNewTitle("Wait in the zone to leave the area");
    }

    private void SetNewTitle(string text)
    {
        _tutorialText.DOKill();

        Sequence seq = DOTween.Sequence().SetTarget(_tutorialText);
        seq.Append(_tutorialText.transform.DOScale(0f, 0.1f).SetTarget(_tutorialText));
        seq.AppendCallback(() => _tutorialText.text = text);
        seq.Append(_tutorialText.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack).SetTarget(_tutorialText));
    }
}
