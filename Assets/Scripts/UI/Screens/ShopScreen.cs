using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
    
    [Header("Tabs")]
    [SerializeField] private List<ItemTypeTab> tabs;
    [SerializeField] private RectTransform tabsFocus;
    [SerializeField] private float tabsFocusAnimaDuration = 0.5f;

    [Header("Item Buttons")] 
    [SerializeField] private RectTransform itemButtonsRoot;
    [SerializeField] private ShopItemButton itemButtonPrefab;

    [Header("Preview")] 
    [SerializeField] private Camera previewCamera;
    [SerializeField] private RawImage previewImage;
    
    [InjectService] private IItemHolderService _itemHolderService;
    [InjectService] private UIService _uiService;

    private ItemTypes _currentItemTypes = ItemTypes.Hats;
    private RenderTexture _previewTexture;
    
    protected override void OnBeforeOpen(UIObject.Args screenOpenProperties)
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        
        exitButton.onClick.AddListener(OnExitButtonClicked);

        SetPreview();
        SubscribeTabs();
    }

    private void SubscribeTabs()
    {
        tabs.ForEach(tab =>
        {
            tab.TabButton.onClick.AddListener(() =>
            {
                tabsFocus.DOKill();
                tabsFocus.SetParent(tab.TabButton.transform);
                tabsFocus.localScale = Vector3.one;
                tabsFocus.anchorMin = new Vector2(0f, 0f);
                tabsFocus.anchorMax = new Vector2(1f, 0f);
                
                float height = tabsFocus.sizeDelta.y;
                Vector2 minStart = tabsFocus.offsetMin; 
                Vector2 maxStart = tabsFocus.offsetMax; 
                DOTween.To(() => 0f, (percent) =>
                {
                    bool moveRight = tabsFocus.offsetMin.x < 0;
                    float percentB = Mathf.Clamp01(percent * 2f);
                    float percentA = Mathf.Clamp01((percent - 0.5f) * 2f); 
                    
                    tabsFocus.offsetMin = Vector2.LerpUnclamped(minStart, Vector2.zero, moveRight ? percentA : percentB);
                    tabsFocus.offsetMax = Vector2.LerpUnclamped(maxStart, Vector2.zero, moveRight ? percentB : percentA);
                    tabsFocus.sizeDelta = new Vector2(tabsFocus.sizeDelta.x, height);
                }, 1f, tabsFocusAnimaDuration)
                    .SetTarget(tabsFocus)
                    .SetEase(Ease.OutQuad);
                
                _currentItemTypes = tab.ItemType;
                BuildItems(); 
            });
        });

        tabs.First().TabButton.onClick.Invoke();
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
                    Instantiate(itemButtonPrefab, itemButtonsRoot).Build(type, x));
            }
        }
    }
    
    private void OnExitButtonClicked()
    {
        _uiService.Back();
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
