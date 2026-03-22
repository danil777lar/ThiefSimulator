using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Larje.Character;
using Larje.Core;
using Larje.Core.Services;
using TMPro;
using UnityEngine;
using UnityEngine.Timeline;

public class ThiefTutorial : MonoBehaviour
{
    [SerializeField] private OffscreenMarker markerPrefab;

    [Header("Lock Step")] 
    [SerializeField] private Transform lockMarker;
    [SerializeField] private MiniGameLauncher lockMiniGame;
    
    [Header("Enemy Step")] 
    [SerializeField] private Character enemy;

    [Header("Loot Step")] 
    [SerializeField] private List<Sellable> loot;

    [InjectService] private IAnalyticsService _analyticsService;

    private bool _minProgressAchieved;
    private bool _fullProgressAchieved;
    private bool _showMarker;
    private bool _levelPlaying;
    
    private bool _isEnemyStep;
    private bool _isLootStep;
    
    private CharacterAttack _playerAttack;
    private CharacterCarry3D _playerCarry;
    private ThiefLevel _level;
    private Transform _markerTarget;
    private Transform _markerCenter;
    private OffscreenMarker _markerInstance;
    private TextMeshProUGUI _tutorialText;
    
    public void OnLevelEvent()
    {
        // if (levelEvent is LevelEventProgressComplete progressCompleteEvent)
        // {
        //     if (progressCompleteEvent.Type == LevelEventProgressComplete.ProgressType.Min)
        //     {
        //         _minProgressAchieved = true;
        //         _analyticsService.SendEvent("Tutorial_Loot_Complete");
        //     }
        //     else if (progressCompleteEvent.Type == LevelEventProgressComplete.ProgressType.Full)
        //     {
        //         _fullProgressAchieved = true;
        //     }
        // }
    }
    
    private void Start()
    {
        DIContainer.InjectTo(this);
        
        _level = GetComponentInParent<ThiefLevel>();
        _tutorialText = GetComponentInChildren<TextMeshProUGUI>();

        Character player = null;//_level.GetComponentsInChildren<Character>().ToList().Find(x => x.CharacterType == Character.CharacterTypes.Player);
        _playerAttack = player.GetComponentInChildren<CharacterAttack>();
        _playerCarry = player.GetComponentInChildren<CharacterCarry3D>();
        
        _markerCenter = player.transform;
        _markerTarget = new GameObject("Tutorial Marker Target").transform;
        _markerTarget.SetParent(transform);
        
        _markerInstance = Instantiate(markerPrefab).Init(_markerTarget, _markerCenter, IsMarkerActive);
        
        StartLockStep();
    }

    private void OnDisable()
    {
        if (_markerInstance != null)
        {
            Destroy(_markerInstance.gameObject);
        }
    }

    private void Update()
    {
        Color targetColor = _level.IsPlaying ? Color.white : Color.clear;
        _tutorialText.color = Color.Lerp(_tutorialText.color, targetColor, Time.deltaTime * 5f);
        
        if (_isEnemyStep)
        {
            EnemyStepUpdate();
        }

        if (_isLootStep)
        {
            LootStepUpdate();   
        }
    }

    private bool IsMarkerActive()
    {
        return _showMarker && _level.IsPlaying;
    }

    private void StartLockStep()
    {
        _showMarker = true;
        _tutorialText.text = "Go to the lock to start unlocking";
        
        _markerTarget.position = lockMarker.position;
        lockMiniGame.eventOnComplete.AddListener(() => 
        {
            StartEnemyStep();
            _analyticsService.SendEvent("Tutorial_Lock_Complete");
        });
    }
    
    private void StartEnemyStep()
    {
        _showMarker = true;
        _isEnemyStep = true;
        _markerTarget.position = enemy.transform.position;
    }

    private void EnemyStepUpdate()
    {
        _tutorialText.text = _playerAttack.HasTarget ? "Wait in the red area to attack" : "Slowly go to the enemy";
        Health enemyHealth = enemy.GetComponent<Health>();
        if (enemyHealth.CurrentHealth <= 0)
        {
            _isEnemyStep = false;
            StartLootStep();    
            _analyticsService.SendEvent("Tutorial_Enemy_Complete");
        }
    }

    private void StartLootStep()
    {
        _showMarker = true;
        _isLootStep = true;
        _tutorialText.text = "Grab the loot";
    }

    private void LootStepUpdate()
    {
        Sellable markedLoot = loot.Find(x => !x.InSaleProcess && !x.InSaleOrder);
        if (markedLoot)
        {
            _markerTarget.position = markedLoot.transform.position;
        }
        
        if (!_minProgressAchieved && !_fullProgressAchieved)
        {
            _tutorialText.text = _playerCarry.HasCarryable ? "Take loot to the car" : "Pick up the loot";
            _showMarker = !_playerCarry.HasCarryable;
        }
        
        if (_minProgressAchieved)
        {
            _tutorialText.text = "Pick up the remaining loot or get into the car";
            _showMarker = !_playerCarry.HasCarryable;
        }
        
        if (_fullProgressAchieved)
        {
            _showMarker = false;
            _tutorialText.text = "Get into the car";
        }
    }
}
