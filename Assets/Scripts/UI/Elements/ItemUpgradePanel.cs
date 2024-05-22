using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
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
    [SerializeField] private Image upgradeLockIcon;
    [SerializeField] private TextMeshProUGUI upgradeButtonText;

    [InjectService] private DataService _dataService;
    [InjectService] private UIService _uiService;
    [InjectService] private ICurrencyService _currencyService;
    [InjectService] private IItemHolderService _itemHolderService;
    
    private ItemType _itemType;
    private ThiefItem _item;
    private UpgradeProcessor _upgrade;
    private ItemUpgradeData _data;

    public void Build(ItemType itemType, ThiefItem item, UpgradeProcessor upgrade)
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        
        _itemType = itemType;
        _item = item;
        _upgrade = upgrade;
        _data = _dataService.Data.GetItemUpgradeData(_item.Name, _itemType, _upgrade.UpgradeType);
        
        icon.sprite = upgrade.Icon;
        upgradeName.text = upgrade.DisplayName;
        upgradeDescription.text = upgrade.GetDescription(0);
        
        UpdateUpgradeButton();
        BuildProgressSlider();
    }

    private void UpdateUpgradeButton()
    {
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
     
        bool isMaxLevel = _data.Level >= _upgrade.MaxLevel;
        bool isUnlocked = _itemHolderService.IsItemUnlocked(_itemType, _item.Name);
        upgradeButton.interactable = isUnlocked && !isMaxLevel;
        upgradeLockIcon.gameObject.SetActive(!isUnlocked);
        upgradeButtonText.gameObject.SetActive(isUnlocked);
        
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
            _data.Level++;
            _dataService.Save();
            progressBar.value = _data.Level;
            
            UpdateUpgradeButton();
        }
        else
        {
            UIToast.Args toast = new UIToast.Args(UIToastType.Info, "Not enough coins");
            _uiService.GetProcessor<UIToastProcessor>().OpenToast(toast);
        }
    }

    private int GetCurrentUpgradePrice()
    {
        float price = _upgrade.BaseLevelPrice * Mathf.Pow(_upgrade.LevelPriceMultiplier, _data.Level);
        return Mathf.RoundToInt(price);
    }
}
