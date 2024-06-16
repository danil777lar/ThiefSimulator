using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using ProjectConstants;
using Unity.VisualScripting;
using UnityEngine;

[BindService(typeof(UpgradesService))]
public class UpgradesService : Service
{
    [SerializeField] private UpgradesServiceConfig config;
    
    public override void Init()
    {
        
    }
    
    public void SpawnUpgrade(UpgradeType upgradeType, int level, Transform parent)
    {
        UpgradeProcessor processor = config.GetUpgradeProcessor(upgradeType);
        if (processor == null)
        {
            return;
        }
        
        UpgradeProcessor upgrade = Instantiate(processor, parent);
        
    }
}
