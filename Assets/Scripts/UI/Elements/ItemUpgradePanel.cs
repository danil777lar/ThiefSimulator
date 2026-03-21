using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUpgradePanel : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI upgradeName;
    [SerializeField] private TextMeshProUGUI upgradeDescription;
    [Header("Progress")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private Image progressBarDivider;
    [Space]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button upgradeAdButton;
    [SerializeField] private Button maxLevelButton;
    [SerializeField] private Image upgradeLockIcon;
    [SerializeField] private TextMeshProUGUI upgradeButtonText;

    [InjectService] private DataService _dataService;
    [InjectService] private UIService _uiService;
    [InjectService] private UpgradesService _upgradesService;
    [InjectService] private ICurrencyService _currencyService;
    [InjectService] private IItemHolderService _itemHolderService;
    [InjectService] private IAdsService _adsService;
    
    private UpgradeProcessor _upgrade;
    private ItemUpgradeData _data;
    private Func<bool> _unlocked;

    public void Build(UpgradeType upgradeType, ItemUpgradeData data, Func<bool> unlocked)
    {
        DIContainer.InjectTo(this);
        
        _upgrade = _upgradesService.GetUpgradePrefab(upgradeType);
        _data = data;
        _unlocked = unlocked;
        
        icon.sprite = _upgrade.Icon;
        upgradeName.text = _upgrade.DisplayName;
        upgradeDescription.text = _upgrade.GetDescription(0);
        
        UpdateUpgradeButtons();
        BuildProgressSlider();
    }

    private void UpdateUpgradeButtons()
    {
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        
        upgradeAdButton.onClick.RemoveAllListeners();
        upgradeAdButton.onClick.AddListener(OnUpgradeButtonAdClicked);
     
        bool isMaxLevel = _data.Level >= _upgrade.MaxLevel;
        bool isUnlocked = _unlocked.Invoke();
        bool isEnoughMoney = _currencyService.GetCurrency(CurrencyType.Coins, CurrencyPlacementType.Global) >= GetCurrentUpgradePrice();
        
        upgradeButton.interactable = isUnlocked;
        upgradeLockIcon.gameObject.SetActive(!isUnlocked);
        upgradeButtonText.gameObject.SetActive(isUnlocked);
        
        upgradeButton.gameObject.SetActive(!isMaxLevel && isEnoughMoney);
        upgradeAdButton.gameObject.SetActive(!isMaxLevel && !isEnoughMoney);
        maxLevelButton.gameObject.SetActive(isMaxLevel);
        
        upgradeButtonText.text = isMaxLevel ? "MAX LEVEL" : $"UPGRADE\n{GetCurrentUpgradePrice()}<sprite index=0>";
    }

    private void BuildProgressSlider()
    {
        progressBar.maxValue = _upgrade.MaxLevel;
        progressBar.value = _data.Level;
        
        progressBarDivider.gameObject.SetActive(false);
        foreach (Transform child in progressBarDivider.transform.parent)
        {
            if (child.gameObject.activeSelf)
            {
                Destroy(child.gameObject);
            }
        }
        for (int i = 0; i < _upgrade.MaxLevel; i++)
        {
            Image image = Instantiate(progressBarDivider, progressBarDivider.transform.parent);
            image.gameObject.SetActive(true);
            image.color = image.color.SetAlpha(i == 0f ? 0f : 1f);
        }
    }

    private void OnUpgradeButtonClicked()
    {
        if (_data.Level >= _upgrade.MaxLevel)
        {
            return;
        }
        
        if (_currencyService.TrySpendCurrency(CurrencyType.Coins, CurrencyPlacementType.Global, GetCurrentUpgradePrice()))
        {
            Upgrade();
        }
        else
        {
            UIToast.Args toast = new UIToast.Args(UIToastType.Info, "Not enough coins");
            _uiService.GetProcessor<UIToastProcessor>().OpenToast(toast);
        }
    }
    
    private void OnUpgradeButtonAdClicked()
    {
        if (_data.Level >= _upgrade.MaxLevel)
        {
            return;
        }
        
        _adsService.ShowRewarded(() => {}, () => { }, 
            Upgrade, () => { });
    }

    private void Upgrade()
    {
        _data.Level++;
        _dataService.SaveGameData();
        progressBar.value = _data.Level;
            
        UpdateUpgradeButtons();
    }

    private int GetCurrentUpgradePrice()
    {
        float price = _upgrade.BaseLevelPrice * Mathf.Pow(_upgrade.LevelPriceMultiplier, _data.Level);
        return Mathf.RoundToInt(price);
    }
}
