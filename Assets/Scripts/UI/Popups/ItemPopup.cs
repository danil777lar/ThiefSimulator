using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemPopup : UIPopup
{
    [SerializeField] private Image icon;
    
    [Header("Buttons")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button removeButton;

    [InjectService] private IItemHolderService _itemHolderService;

    private Args _args;
    
    protected override void OnBeforeOpen(UIObject.Args args)
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
    
        if (args is Args itemArgs)
        {
            _args = itemArgs;
            icon.sprite = _args.Item.Icon;
            icon.preserveAspect = true;
        }
        
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
        bool isItemEquip = _itemHolderService.TryGetCurrentItem(out Item currentItem, _args.ItemType) 
                           && currentItem.Name == _args.Item.Name;
        
        equipButton.gameObject.SetActive(!isItemEquip);
        removeButton.gameObject.SetActive(isItemEquip);
    }
    
    private void OnEquipButtonClicked()
    {
        _itemHolderService.UnlockItem(_args.ItemType, _args.Item.Name);
        _itemHolderService.SetCurrentItem(_args.ItemType, _args.Item.Name);
        Close();
    }
    
    private void OnRemoveButtonClicked()
    {
        _itemHolderService.SetCurrentItem(_args.ItemType, string.Empty);
        Close();
    }

    public class Args : UIPopup.Args
    {
        public readonly ItemType ItemType;
        public readonly Item Item;
        
        public Args(ItemType type, Item item) : base(UIPopupType.Item)
        {
            ItemType = type;
            Item = item;
        }

        public Args(ItemType type, Item item, UIPopupCombinationType combinationType) : base(UIPopupType.Item, combinationType)
        {
            ItemType = type;
            Item = item;
        }
    }
}
