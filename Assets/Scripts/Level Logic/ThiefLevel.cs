using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Services;
using MoreMountains.TopDownEngine;
using ProjectConstants;
using Unity.AI.Navigation;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class ThiefLevel : LevelProcessor
{
    [SerializeField] private int moneyForWin = 100;
    [Header("Grid")]
    [SerializeField, Min(1f)] private float gridSize = 2f;
    [SerializeField, Min(1f)] private float maxPointDistance = 1f;
    [Header("Gizmos")] 
    [SerializeField] private Color gizmoColor;
    [SerializeField] private float gizmoSize;
    
    [InjectService] private ICurrencyService _currencyService;

    public float Progress { get; private set; }
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

    public override LevelData GetLevelData()
    {
        return null;
    }
    
    private void Start()
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        
        GrabCurrencyService();
        BuildNavmesh();
        GrabCharacters();
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
        float oldProgress = Progress;
        Progress = (float)_currencyService.GetCurrency(CurrencyType.Coins, CurrencyPlacementType.Level) / 
                   (float)moneyForWin;
        if (oldProgress < 1f && Progress >= 1f)
        {
            SendEvent(new LevelEventProgressComplete());
        }
    }
}
