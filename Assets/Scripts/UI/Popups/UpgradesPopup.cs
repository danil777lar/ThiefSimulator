using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesPopup : UIPopup
{
    [Space(40f)] 
    [SerializeField] private Button buttonClose;
    [Space]
    [SerializeField] private ItemUpgradePanel upgradePanelPrefab;
    [SerializeField] private Transform content;
    
    [InjectService] private UpgradesService _upgradesService;
    
    protected override void OnBeforeOpen(UIObject.Args args)
    {
        DIContainer.InjectTo(this);
        
        buttonClose.onClick.AddListener(Close);

        BuildUpgrades();
    }

    private void BuildUpgrades()
    {
        content.DestroyAllChildren();

        foreach (KeyValuePair<UpgradeType, ItemUpgradeData> upgrade in _upgradesService.GetPlayerGlobalUpgrades()){
            
            Instantiate(upgradePanelPrefab, content).Build(upgrade.Key, upgrade.Value, () => true);
        }
    }
    
    public new class Args : UIPopup.Args
    {
        public Args() : base(UIPopupType.Upgrades)
        {
        }

        public Args(UIPopupCombinationType combinationType) : base(UIPopupType.Upgrades, combinationType)
        {
        }
    }
}
