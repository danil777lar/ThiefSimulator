using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Character;
using Larje.Core;
using Larje.Core.Services;
using ProjectConstants;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class ThiefLevel : LevelProcessor
{
    [Header("Base")] 
    [SerializeField] private float startDelay;

    [Header("Grid")] 
    [SerializeField, Min(1f)] private float gridSize = 2f;
    [SerializeField, Min(1f)] private float maxPointDistance = 1f;

    [Header("Gizmos")] 
    [SerializeField] private Color gizmoColor;
    [SerializeField] private float gizmoSize;
    
    [InjectService] private GameEventService _gameEventService;
    [InjectService] private IGameStateService _gameStateService;
    [InjectService] private ICurrencyService _currencyService;

    private int _lootInitialCount;
    private int _lootSoldCount;
    private LevelData _levelData;

    public float Progress { get; private set; }
    public bool IsPlaying => IsLevelPlaying;
    public IReadOnlyList<Vector3> Points { get; private set; }
    public IReadOnlyList<Character> Characters { get; private set; }
    
    public override void TryStartLevel(StartData data)
    {
        StartLevel(data);
    }

    public override void TryStopLevel(StopData data)
    {
        StopLevel(data);
    }

    public override LevelProcessor.LevelData GetLevelData()
    {
        return _levelData;
    }
    
    private void Start()
    {
        DIContainer.InjectTo(this);

        _levelData = new LevelData(this);
        
        GrabCurrencyService();
        BuildNavmesh();
        GrabCharacters();
        GrabLoot();
    }

    private void OnDisable()
    {
    }

    private void OnDrawGizmos()
    {
        if (Points != null)
        {
            Gizmos.color = gizmoColor;
            foreach (Vector3 point in Points)
            {
                Gizmos.DrawSphere(point, gizmoSize);   
            }
        }
    }

    private void GrabCurrencyService()
    {
        _currencyService.SetCurrency(CurrencyType.Coins, CurrencyPlacementType.Level, 0);
    }

    private void GrabLoot()
    {
        List<Sellable> sellables = GetComponentsInChildren<Sellable>().ToList();

        _lootInitialCount = sellables.Count;
        foreach (Sellable s in sellables)
        {
            s.EventSold += () => 
            {
                _lootSoldCount++;
                UpdateLootProgress();
            };
        }
    }

    private void UpdateLootProgress()
    {
        Progress = (float)_lootSoldCount / (float)_lootInitialCount;
        if (_lootSoldCount >= _lootInitialCount)
        {
            _gameEventService.SendEvent(new LevelEventProgressComplete());
        }
    }

    [ContextMenu("Build Navmesh")]
    private void BuildNavmesh()
    {
        StartCoroutine(BuildNavmeshCo());
    }

    private void BuildPoints()
    {
        List<Vector3> points = new List<Vector3>();
        for (float x = -100f; x <= 100f; x += gridSize)
        {
            for (float z = -100f; z <= 100f; z += gridSize)
            {
                Vector3 point = transform.position + new Vector3(x, 0, z);
                if (NavMesh.SamplePosition(point, out NavMeshHit hit, maxPointDistance, NavMesh.AllAreas))
                {
                    points.Add(hit.position);
                }
            }   
        }

        Points = points.AsReadOnly();
    }

    private void GrabCharacters()
    {
        Characters = GetComponentsInChildren<Character>();
    }
    
    
    private IEnumerator BuildNavmeshCo()
    {
        for (int i = 0; i < 4; i++)
        {
            yield return null;   
        }
        
        List<NavMeshSurface> surfaces = GetComponents<NavMeshSurface>().ToList();
        surfaces.ForEach(x => x.BuildNavMesh());
        BuildPoints();
    }
    
    public new class LevelData : LevelProcessor.LevelData
    {
        private readonly ThiefLevel _level;

        public float Progress => _level.Progress;
        
        public LevelData (ThiefLevel level)
        {
            _level = level;
        }
    }
}
