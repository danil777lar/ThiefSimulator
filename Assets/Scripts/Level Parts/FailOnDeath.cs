using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using MoreMountains.TopDownEngine;
using ProjectConstants;
using UnityEngine;

public class FailOnDeath : MonoBehaviour
{
    [InjectService] private ILevelManagerService _levelManagerService;
    
    private Health _health;

    private void Start()
    {
        DIContainer.InjectTo(this);
        
        _health = GetComponentInParent<Health>();
        _health.OnDeath += Fail;
    }

    private void Fail()
    {
        _levelManagerService.TryStopCurrentLevel(new LevelProcessor.StopData(false, LevelStopType.Fail));
    }
}
