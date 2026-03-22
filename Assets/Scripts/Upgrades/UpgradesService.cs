using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core;
using Larje.Core.Services;
using ProjectConstants;
using UnityEngine;

[BindService(typeof(UpgradesService))]
public class UpgradesService : Service
{
    private const string PLAYER_UPGRADES_KEY = "player_global";
    
    [SerializeField] private UpgradesServiceConfig config;
    [Space]
    [SerializeField] private List<UpgradeType> playerGlobalUpgrades;

    [InjectService] private IDataService _dataService;
    [InjectService] private ICurrencyService _currencyService;
    
    public override void Init()
    {
        
    }

    public bool CanMakeSomeUpgrade()
    {
        foreach (UpgradeType upgrade in playerGlobalUpgrades)
        {
            UpgradeProcessor processor = config.GetUpgradeProcessor(upgrade);
            ItemUpgradeData data = _dataService.GameData.GetItemUpgradeData(PLAYER_UPGRADES_KEY, upgrade);
            
            int price = Mathf.RoundToInt(processor.BaseLevelPrice * Mathf.Pow(processor.LevelPriceMultiplier, data.Level));
            
            bool canUpgrade = _currencyService.CheckEnoughCurrency(CurrencyType.Coins, CurrencyPlacementType.Global, price);
            canUpgrade &= data.Level < processor.MaxLevel;
            if (canUpgrade)
            {
                return true;
            }
        }
        
        return false;
    }

    public Dictionary<UpgradeType, ItemUpgradeData> GetPlayerGlobalUpgrades()
    {
        Dictionary<UpgradeType, ItemUpgradeData> upgrades = new Dictionary<UpgradeType, ItemUpgradeData>();
        foreach (UpgradeType upgrade in playerGlobalUpgrades)
        {
            ItemUpgradeData data = _dataService.GameData.GetItemUpgradeData(PLAYER_UPGRADES_KEY, upgrade);
            upgrades.Add(upgrade, data);
        }
        
        return upgrades;
    }

    public UpgradeProcessor GetUpgradePrefab(UpgradeType upgradeType)
    {
        return config.GetUpgradeProcessor(upgradeType);
    }
    
    public UpgradeProcessor SpawnUpgrade(UpgradeType upgradeType, int level, Transform parent)
    {
        UpgradeProcessor processor = config.GetUpgradeProcessor(upgradeType);
        if (processor == null)
        {
            return null;
        }

        UpgradeProcessor instance = parent.GetComponentsInChildren<UpgradeProcessor>().ToList()
            .Find(x => x.UpgradeType == upgradeType);
        if (instance != null)
        {
            instance.Combine(level);
        }
        else
        {
            instance = Instantiate(processor, parent);
            instance.Init(level);   
        }

        return instance;
    }
}
