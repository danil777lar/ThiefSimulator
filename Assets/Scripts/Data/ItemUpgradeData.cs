using System;
using System.Collections;
using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;


namespace Larje.Core.Services
{
    public partial class GameData
    {
        public List<ItemUpgradeData> ItemUpgrades;

        public ItemUpgradeData GetItemUpgradeData(string itemName, ItemType itemType, UpgradeType upgradeType)
        {
            ItemUpgrades ??= new List<ItemUpgradeData>();
            ItemUpgradeData data = ItemUpgrades.Find(x => x.ItemName == itemName && x.ItemType == itemType && x.UpgradeType == upgradeType);
            if (data == null)
            {
                data = new ItemUpgradeData
                {
                    ItemName = itemName,
                    ItemType = itemType,
                    UpgradeType = upgradeType,
                    Level = 0
                };
                ItemUpgrades.Add(data);
            }

            return data;
        }
    }
}

[Serializable]
public class ItemUpgradeData
{
    public string ItemName;
    public ItemType ItemType;
    public UpgradeType UpgradeType;

    public int Level;
}
