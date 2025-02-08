using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemButton : MonoBehaviour, IItemQualityBackgroundUser
{
    [SerializeField] private Image icon;
    [SerializeField] private GameObject lockIcon;
    
    [InjectService] private ItemHolderService _itemHolderService;
    [InjectService] private UIService _uiService;

    private ItemType _itemType;
    private Item _item;
    private Button _button; 
    
    public ThiefItem Item => _item as ThiefItem;
    
    public event Action EventItemChanged;
    
    public void Build(ItemType type, Item item)
    {
        DIContainer.InjectTo(this);

        _itemType = type;
        _item = item;
        EventItemChanged?.Invoke();
        
        icon.sprite = item.Icon;
        icon.preserveAspect = true;
        
        lockIcon.SetActive(!_itemHolderService.IsItemUnlocked(_itemType, _item.Name));
        
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClicked);
    }
    
    private void OnButtonClicked()
    {
        UIPopup.Args args = new ItemPopup.Args(_itemType, _item);
        _uiService.GetProcessor<UIPopupProcessor>().OpenPopup(args);
    }
}
