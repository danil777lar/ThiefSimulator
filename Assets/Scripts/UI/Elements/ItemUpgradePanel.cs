using System.Collections;
using System.Collections.Generic;
using ProjectConstants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUpgradePanel : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI upgradeName;
    [SerializeField] private TextMeshProUGUI upgradeDescription;
    [Space]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TextMeshProUGUI upgradeButtonText;

    public void Build(ItemType itemType, ThiefItem item, UpgradeProcessor upgrade)
    {
        icon.sprite = upgrade.Icon;
        upgradeName.text = upgrade.DisplayName;
        upgradeDescription.text = upgrade.GetDescription(0);
    }
}
