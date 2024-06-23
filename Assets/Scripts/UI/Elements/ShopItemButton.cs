using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private List<ItemBackground> backgrounds;
    
    [InjectService] private ItemHolderService _itemHolderService;
    [InjectService] private UIService _uiService;

    private ItemType _itemType;
    private Item _item;
    private Button _button; 
    
    public void Build(ItemType type, Item item)
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);

        _itemType = type;
        _item = item;
        
        icon.sprite = item.Icon;
        icon.preserveAspect = true;
        
        lockIcon.SetActive(!_itemHolderService.IsItemUnlocked(_itemType, _item.Name));
        
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClicked);
        
        if (item is ThiefItem thiefItem)
        {
            backgrounds.ForEach(bg => bg.Background.SetActive(bg.Quality == thiefItem.Quality));
        }
    }
    
    private void OnButtonClicked()
    {
        UIPopup.Args args = new ItemPopup.Args(_itemType, _item);
        _uiService.GetProcessor<UIPopupProcessor>().OpenPopup(args);
    }

    [Serializable]
    private class ItemBackground
    {
        [field: SerializeField] public ItemQuality Quality { get; private set; }
        [field: SerializeField] public GameObject Background { get; private set; }
    }
}
