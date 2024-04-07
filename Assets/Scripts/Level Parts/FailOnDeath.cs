using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using MoreMountains.TopDownEngine;
using ProjectConstants;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class FailOnDeath : MonoBehaviour
{
    [InjectService] private ILevelManagerService _levelManagerService;
    
    private Health _health;

    private void Start()
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        
        _health = GetComponent<Health>();
        _health.OnDeath += Fail;
    }

    private void Fail()
    {
        _levelManagerService.TryStopCurrentLevel(new LevelProcessor.StopData(LevelStopType.Fail));
    }
}
