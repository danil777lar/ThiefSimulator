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
        
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClicked);
    }
    
    private void OnButtonClicked()
    {
        UIPopup.Args args = new ItemPopup.Args(_itemType, _item);
        _uiService.GetProcessor<UIPopupProcessor>().OpenPopup(args);
    }
}
