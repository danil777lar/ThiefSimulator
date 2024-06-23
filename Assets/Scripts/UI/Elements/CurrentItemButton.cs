using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    [SerializeField] private List<ItemBackground> backgrounds;
    
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

    private void OnDestroy()
    {
        _itemHolderService.EventCurrentItemChanged -= UpdateItem;
    }

    private void UpdateItem()
    {
        _itemHolderService.TryGetCurrentItem(out var newItem, itemType);

        if (newItem != _item)
        {
            _item = newItem;
            icon.sprite = _item?.Icon;
            icon.gameObject.SetActive(_item != null);
            placeholder.gameObject.SetActive(_item == null);

            _button.interactable = _item != null;

            backgrounds.ForEach(bg =>
            {
                if (_item is ThiefItem thiefItem)
                {
                    bg.Background.SetActive(bg.Quality == thiefItem.Quality);
                }
                else
                {
                    bg.Background.SetActive(false);
                }
            });

            PlayUpdateAnimation();
        }
    }

    private void OnButtonClicked()
    {
        UIPopup.Args args = new ItemPopup.Args(itemType, _item);
        _uiService.GetProcessor<UIPopupProcessor>().OpenPopup(args);        
    }

    private void PlayUpdateAnimation()
    {
        this.DOKill();
        transform.DOScale(0.8f, 0.1f)
            .SetTarget(this)
            .OnComplete(() =>
            {
                transform.DOScale(1f, 0.25f)
                    .SetEase(Ease.OutBack)
                    .SetTarget(this);
            });
    }
    
    [Serializable]
    private class ItemBackground
    {
        [field: SerializeField] public ItemQuality Quality { get; private set; }
        [field: SerializeField] public GameObject Background { get; private set; }
    }
}
