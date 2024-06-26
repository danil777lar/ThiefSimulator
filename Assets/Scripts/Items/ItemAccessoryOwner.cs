using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Services;
using MoreMountains.TopDownEngine;
using ProjectConstants;
using UnityEngine;

public class ItemAccessoryOwner : MonoBehaviour
{
    [SerializeField] private bool useUpgrades = true;
    
    [InjectService] private IItemHolderService _itemsService;
    [InjectService] private UpgradesService _upgradesService;
    [InjectService] private DataService _dataService;

    private GameObject _root;
    private List<UpgradeProcessor> _upgrades = new List<UpgradeProcessor>();
    private List<ItemAccessory> _accessories;
    
    private void Start()
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        
        GrabAccessories();
        UpdateAccessories();
        
        _itemsService.EventCurrentItemChanged += UpdateAccessories;
    }

    private void OnDestroy()
    {
        _itemsService.EventCurrentItemChanged -= UpdateAccessories;
    }

    private void GrabAccessories()
    {
        Character character = GetComponentInParent<Character>();
        _root = character != null ? character.gameObject : gameObject;
        _accessories = _root.GetComponentsInChildren<ItemAccessory>(true).ToList();
    }
    
    private void UpdateAccessories()
    {
        _upgrades.ForEach(x => x.Remove());
        _upgrades = new List<UpgradeProcessor>();
        
        foreach (ItemAccessory accessory in _accessories)
        {
            string key = _itemsService.TryGetCurrentItem(out Item currentItem, accessory.ItemType) ? currentItem.Name : "";
            bool isActive = accessory.Key == key;
            accessory.gameObject.SetActive(isActive);
            if (isActive)
            {
                SpawnUpgrades(currentItem, accessory.ItemType);
            }
        }
    }

    private void SpawnUpgrades(Item item, ItemType itemType)
    {
        if (!useUpgrades)
        {
            return;
        }
        
        if (item is ThiefItem thiefItem)
        {
            foreach (UpgradeType upgrade in thiefItem.Upgrades)
            {
                ItemUpgradeData data = _dataService.Data.GetItemUpgradeData(thiefItem.Name, itemType, upgrade);
                UpgradeProcessor upgradeInstance = _upgradesService.SpawnUpgrade(upgrade, data.Level, _root.transform);
                if (!_upgrades.Contains(upgradeInstance))
                {
                    _upgrades.Add(upgradeInstance);
                }
            }
        }
    }
}
