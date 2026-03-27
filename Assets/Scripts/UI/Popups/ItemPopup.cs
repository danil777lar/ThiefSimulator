using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemPopup : UIPopup
{
    [SerializeField] private Image icon;
    [SerializeField] private Image frame;
    [SerializeField] private TextMeshProUGUI qualityTitle;
    [SerializeField] private TextMeshProUGUI nameTitle;
    [SerializeField] private TextMeshProUGUI unlockInGameTitle;
    [SerializeField] private List<QualityOptions> qualityOptions;

    [Header("Buttons")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button removeButton;

    [InjectService] private IItemHolderService _itemHolderService;
    [InjectService] private IDataService _dataService;

    private Args _args;
    
    protected override void OnBeforeOpen(UIObject.Args args)
    {
        DIContainer.InjectTo(this);
    
        if (args is Args itemArgs)
        {
            _args = itemArgs;
            icon.sprite = _args.Item.Icon;
            icon.preserveAspect = true;
        }

        frame.sprite = GetQualityOptions().Frame;
        qualityTitle.text = _args.Item.Quality.ToString(); 
        qualityTitle.color = GetQualityOptions().QualityTitleColor;
        nameTitle.text = _args.Item.DisplayName; 
        nameTitle.color = GetQualityOptions().NameTitleColor;
        
        closeButton.onClick.AddListener(Close);
        equipButton.onClick.AddListener(OnEquipButtonClicked);   
        removeButton.onClick.AddListener(OnRemoveButtonClicked);
        
        _itemHolderService.EventCurrentItemChanged += UpdateUI;
        UpdateUI();
    }

    private void OnDestroy()
    {
        _itemHolderService.EventCurrentItemChanged -= UpdateUI;
    }

    private void UpdateUI()
    {
        bool isItemUnlock = _itemHolderService.IsItemUnlocked(_args.ItemType, _args.Item.Name);
        bool isItemEquip = _itemHolderService.TryGetCurrentItem(out Item currentItem, _args.ItemType) 
                           && currentItem.Name == _args.Item.Name;
        
        unlockInGameTitle.gameObject.SetActive(!isItemUnlock);
        equipButton.gameObject.SetActive(isItemUnlock && !isItemEquip);
        removeButton.gameObject.SetActive(isItemUnlock && isItemEquip);
    }
    
    private void OnEquipButtonClicked()
    {
        
        _itemHolderService.SetCurrentItem(_args.ItemType, _args.Item.Name);
        Close();
    }
    
    private void OnRemoveButtonClicked()
    {
        _itemHolderService.SetCurrentItem(_args.ItemType, string.Empty);
        Close();
    }

    private QualityOptions GetQualityOptions()
    {
        return qualityOptions.Find(x => x.Quality == _args.Item.Quality);
    }

    [Serializable]
    private class QualityOptions
    {
        [field: SerializeField] public ItemQuality Quality;
        [field: SerializeField] public Color QualityTitleColor;
        [field: SerializeField] public Color NameTitleColor;
        [field: SerializeField] public Sprite Frame;
    }

    public class Args : UIPopup.Args
    {
        public readonly ItemType ItemType;
        public readonly ThiefItem Item;
        
        public Args(ItemType type, Item item) : base(UIPopupType.Item)
        {
            ItemType = type;
            Item = item as ThiefItem;
        }

        public Args(ItemType type, Item item, UIPopupCombinationType combinationType) : base(UIPopupType.Item, combinationType)
        {
            ItemType = type;
            Item = item as ThiefItem;
        }
    }
}
