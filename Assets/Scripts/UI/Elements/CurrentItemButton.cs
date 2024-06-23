using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.UI;

public class CurrentItemButton : MonoBehaviour, IItemQualityBackgroundUser
{
    [SerializeField] private ItemType itemType;
    [Space] 
    [SerializeField] private Image icon;
    [SerializeField] private Image placeholder;
    
    [InjectService] private IItemHolderService _itemHolderService;
    [InjectService] private UIService _uiService;

    private Item _item;
    private Button _button; 
    
    public ThiefItem Item => _item as ThiefItem;
    
    public event Action EventItemChanged;

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
            EventItemChanged?.Invoke();
            
            icon.sprite = _item?.Icon;
            icon.gameObject.SetActive(_item != null);
            placeholder.gameObject.SetActive(_item == null);

            _button.interactable = _item != null;

            PlayUpdateAnimation();
        }
    }

    private void OnButtonClicked()
    {
        if (_item != null)
        {
            UIPopup.Args args = new ItemPopup.Args(itemType, _item);
            _uiService.GetProcessor<UIPopupProcessor>().OpenPopup(args);
        }
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
}
