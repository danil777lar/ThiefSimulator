using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Larje.Core.Services;
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
    [SerializeField] private TextMeshProUGUI upgradeButtonText;

    [InjectService] private DataService _dataService;
    [InjectService] private ICurrencyService _currencyService;
    
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
        
        BuildProgressSlider();
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
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
        _data.Level++;
        _dataService.Save();
        progressBar.value = _data.Level;
    }
}
