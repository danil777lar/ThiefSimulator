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

        public ItemUpgradeData GetItemUpgradeData(string key, UpgradeType upgradeType)
        {
            ItemUpgrades ??= new List<ItemUpgradeData>();
            ItemUpgradeData data = ItemUpgrades.Find(x => x.Key == key && x.UpgradeType == upgradeType);
            if (data == null)
            {
                data = new ItemUpgradeData
                {
                    Key = key,
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
    public string Key;
    public UpgradeType UpgradeType;

    public int Level;
}
