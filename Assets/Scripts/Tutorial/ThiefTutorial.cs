using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MoreMountains.TopDownEngine;
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
    [SerializeField] private List<Sellable> mainLootGroup;
    [SerializeField] private List<Sellable> secondaryLootGroup;

    private bool _showMarker;
    
    private bool _isEnemyStep;
    private bool _isLootStep;
    
    private CharacterAttack _playerAttack;
    private CharacterAttack _playerCarry;
    private ThiefLevel _level;
    private Transform _markerTarget;
    private Transform _markerCenter;
    private OffscreenMarker _markerInstance;
    private TextMeshProUGUI _tutorialText;
    
    private void Start()
    {
        _level = GetComponentInParent<ThiefLevel>();
        _tutorialText = GetComponentInChildren<TextMeshProUGUI>();

        Character player = _level.GetComponentsInChildren<Character>().ToList()
            .Find(x => x.CharacterType == Character.CharacterTypes.Player);
        _playerAttack = player.GetComponentInChildren<CharacterAttack>();
        
        _markerCenter = player.transform;
        _markerTarget = new GameObject("Tutorial Marker Target").transform;
        _markerTarget.SetParent(transform);
        
        _markerInstance = Instantiate(markerPrefab).Init(_markerTarget, _markerCenter, IsMarkerActive);
        
        StartLockStep();
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
        return _showMarker;
    }

    private void StartLockStep()
    {
        _showMarker = true;
        _tutorialText.text = "Go to the lock to start unlocking";
        
        _markerTarget.position = lockMarker.position;
        lockMiniGame.eventOnComplete.AddListener(StartEnemyStep);
    }
    
    private void StartEnemyStep()
    {
        _showMarker = true;
        _isEnemyStep = true;
        _markerTarget.position = enemy.transform.position;
    }

    private void EnemyStepUpdate()
    {
        _tutorialText.text = _playerAttack.HasTarget ? "Wait in trigger to attack" : "Slowly go to the enemy";
        if (enemy.CharacterHealth.CurrentHealth <= 0)
        {
            _isEnemyStep = false;
            StartGoldStep();    
        }
    }

    private void StartGoldStep()
    {
        _showMarker = true;
        _tutorialText.text = "Grab the loot";
    }

    private void LootStepUpdate()
    {
        if (_playerCarry.)
    }
}
