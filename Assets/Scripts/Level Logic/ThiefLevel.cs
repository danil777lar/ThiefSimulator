using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Services;
using MoreMountains.TopDownEngine;
using ProjectConstants;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class ThiefLevel : LevelProcessor
{
    [Header("Win Conditions")]
    [SerializeField, Range(0f, 1f)] private float moneyPercentForWin = 1f;
    [Header("Grid")]
    [SerializeField, Min(1f)] private float gridSize = 2f;
    [SerializeField, Min(1f)] private float maxPointDistance = 1f;
    [Header("Gizmos")] 
    [SerializeField] private Color gizmoColor;
    [SerializeField] private float gizmoSize;
    
    [InjectService] private ICurrencyService _currencyService;

    private float lootTotalPrice;
    private LevelData _levelData;

    public float ProgressTotal { get; private set; }
    public float ProgressForWin { get; private set; }
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
        ServiceLocator.Instance.InjectServicesInComponent(this);

        _levelData = new LevelData(this);
        
        GrabCurrencyService();
        BuildNavmesh();
        GrabCharacters();
        GrabLootTotalPrice();
    }

    private void OnDisable()
    {
        _currencyService.EventCurrencyChanged -= OnCurrencyChanged;
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
        _currencyService.EventCurrencyChanged += OnCurrencyChanged;
        OnCurrencyChanged();        
    }

    private void GrabLootTotalPrice()
    {
        GetComponentsInChildren<Sellable>().ToList().ForEach(x => lootTotalPrice += x.Cost);
    }

    [ContextMenu("Build Navmesh")]
    private void BuildNavmesh()
    {
        List<NavMeshSurface> surfaces = GetComponents<NavMeshSurface>().ToList();
        surfaces.ForEach(x => x.BuildNavMesh());

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
    
    private void OnCurrencyChanged()
    {
        if (lootTotalPrice > 0)
        {
            float oldProgress = ProgressForWin;
            float currentMoney = (float)_currencyService.GetCurrency(CurrencyType.Coins, CurrencyPlacementType.Level);

            ProgressTotal = currentMoney / (float)lootTotalPrice;
            ProgressForWin = Mathf.Clamp01(ProgressTotal / moneyPercentForWin);

            if (oldProgress < 1f && ProgressForWin >= 1f)
            {
                SendEvent(new LevelEventProgressComplete());
            }
        }
    }
    
    public new class LevelData : LevelProcessor.LevelData
    {
        private readonly ThiefLevel _level;

        public float ProgressForWin => _level.ProgressForWin;
        public float ProgressTotal => _level.ProgressTotal;
        
        public LevelData (ThiefLevel level)
        {
            _level = level;
        }
    }
}
