using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Services;
using MoreMountains.TopDownEngine;
using ProjectConstants;
using UnityEngine;

public class ItemAccessoryOwner : MonoBehaviour
{
    [InjectService] private IItemHolderService _itemsService;

    private List<ItemAccessory> _accessories;
    
    private void Start()
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        
        GrabAccessories();
        UpdateAccessories();
        
        _itemsService.EventCurrentItemChanged += UpdateAccessories;
    }

    private void GrabAccessories()
    {
        Character character = GetComponentInParent<Character>();
        GameObject root = character != null ? character.gameObject : gameObject;
        _accessories = root.GetComponentsInChildren<ItemAccessory>(true).ToList();
    }
    
    private void UpdateAccessories()
    {
        foreach (ItemAccessory accessory in _accessories)
        {
            string key = _itemsService.TryGetCurrentItem(out Item currentItem, accessory.ItemType) ? currentItem.Name : "";
            accessory.gameObject.SetActive(accessory.Key == key);
        }
    }
}
