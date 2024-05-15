using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.UI;

public class CurrentItemButton : MonoBehaviour
{
    [SerializeField] private ItemType itemType;
    [Space] 
    [SerializeField] private Image icon;
    [SerializeField] private Image placeholder;
    
    [InjectService] private IItemHolderService _itemHolderService;
    [InjectService] private UIService _uiService;

    private Item _item;
    private Button _button; 

    private void Start()
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);

        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClicked);
        
        _itemHolderService.EventCurrentItemChanged += UpdateItem;
        UpdateItem();
    }

    private void UpdateItem()
    {
        _itemHolderService.TryGetCurrentItem(out _item, itemType);
        
        icon.sprite = _item?.Icon;
        icon.gameObject.SetActive(_item != null);
        placeholder.gameObject.SetActive(_item == null);
        
        _button.interactable = _item != null;
    }

    private void OnButtonClicked()
    {
        UIPopup.Args args = new ItemPopup.Args(itemType, _item);
        _uiService.GetProcessor<UIPopupProcessor>().OpenPopup(args);        
    }
}
