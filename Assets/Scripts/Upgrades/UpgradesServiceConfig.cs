using System.Collections;
using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradesServiceConfig", menuName = "Configs/UpgradesServiceConfig")]
public class UpgradesServiceConfig : ScriptableObject
{
    [SerializeField] private List<UpgradeProcessor> upgradeProcessors;

    public UpgradeProcessor GetUpgradeProcessor(UpgradeType upgradeType)
    {
        UpgradeProcessor upgrade = upgradeProcessors.Find(up => up.UpgradeType == upgradeType);
        if (upgrade == null)
        {
            Debug.LogError($"UpgradeProcessor with UpgradeType {upgradeType} not found");
        }

        return upgrade;
    }
}
