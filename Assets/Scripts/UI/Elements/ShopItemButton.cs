using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using UnityEngine;

public class ShopItemButton : MonoBehaviour
{
    [InjectService] private ItemHolderService _itemHolderService;
    
    private Item _item; 
    
    public void Build(Item item)
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        _item = item;
    }
}
