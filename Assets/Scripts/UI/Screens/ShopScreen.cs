using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using MoreMountains.Tools;
using ProjectConstants;
using UnityEngine;
using UnityEngine.UI;

public class ShopScreen : UIScreen
{
    [Space(40f)] 
    [SerializeField] private Button exitButton;
    [SerializeField] private List<ItemTypeTab> tabs;

    [Header("Item Buttons")] 
    [SerializeField] private RectTransform itemButtonsRoot;
    [SerializeField] private ShopItemButton itemButtonPrefab;

    [Header("Preview")] 
    [SerializeField] private Camera previewCamera;
    [SerializeField] private RawImage previewImage;
    
    [InjectService] private IItemHolderService _itemHolderService;

    private ItemTypes _currentItemTypes = ItemTypes.Hats;
    private RenderTexture _previewTexture;
    
    protected override void OnBeforeOpen(UIObject.Args screenOpenProperties)
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        
        exitButton.onClick.AddListener(OnExitButtonClicked);

        SubscribeTabs();
        SetPreview();
        BuildItems();
    }

    private void SubscribeTabs()
    {
        tabs.ForEach(x =>
        {
            x.TabButton.onClick.AddListener(() =>
            {
                _currentItemTypes = x.ItemType;
                BuildItems(); 
            });
        });
    }

    private void SetPreview()
    {
        _previewTexture = new RenderTexture((int)previewImage.rectTransform.rect.width, 
            (int)previewImage.rectTransform.rect.height, 24);
        previewImage.texture = _previewTexture;
        previewCamera.targetTexture = _previewTexture;
    }

    private void BuildItems()
    {
        itemButtonsRoot.MMDestroyAllChildren();
        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            if (_currentItemTypes.HasFlag((ItemTypes)(int)type))
            {
                _itemHolderService.GetAllItems(type).ForEach(x => 
                    Instantiate(itemButtonPrefab, itemButtonsRoot).Build(x));
            }
        }
    }
    
    private void OnExitButtonClicked()
    {
        Back();
    }

    [Serializable]
    private class ItemTypeTab
    {
        [field: SerializeField] public ItemTypes ItemType { get; private set; }
        [field: SerializeField] public Button TabButton { get; private set; }
    }
    
    public new class Args : UIScreen.Args
    {
        public Args() : base(UIScreenType.Shop)
        {
        }
    }
}
