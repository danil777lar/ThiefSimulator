using System.Collections.Generic;
using System.Linq;
using Larje.Character;
using Larje.Core;
using Larje.Core.Services;
using ProjectConstants;
using UnityEngine;

public class PlayerUpgradesSpawner : MonoBehaviour
{
    [InjectService] private UpgradesService _upgradesService;

    private Transform _root;
    private Dictionary<UpgradeType, UpgradeProcessor> _upgrades = new Dictionary<UpgradeType, UpgradeProcessor>();
    private Dictionary<UpgradeType, ItemUpgradeData> _dataSet;
    
    private void Start()
    {
        DIContainer.InjectTo(this);
        
        _dataSet = _upgradesService.GetPlayerGlobalUpgrades();
        _root = new GameObject("Upgrades").transform; 
        _root.SetParent(GetComponentInParent<Character>().transform);
        
        SpawnUpgrades();
    }

    private void Update()
    {
        SpawnUpgrades();
    }
    
    private void SpawnUpgrades()
    {
        foreach (KeyValuePair<UpgradeType, ItemUpgradeData> upgrade in _dataSet)
        {
            bool needToSpawn = !_upgrades.ContainsKey(upgrade.Key);
            if (!needToSpawn)
            {
                needToSpawn |= _upgrades[upgrade.Key].CurrentLevel != upgrade.Value.Level;
                if (needToSpawn)
                {
                    _upgrades[upgrade.Key].Remove();
                    _upgrades.Remove(upgrade.Key);
                }
            }

            if (needToSpawn)
            {
                UpgradeProcessor upgradeInstance =
                    _upgradesService.SpawnUpgrade(upgrade.Key, upgrade.Value.Level, _root);
                _upgrades.Add(upgrade.Key, upgradeInstance);
            }
        }
    }
}
