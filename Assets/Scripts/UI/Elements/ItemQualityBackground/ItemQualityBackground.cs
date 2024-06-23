using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ItemQualityBackground : MonoBehaviour
{
    [SerializeField] private bool changeActiveState = true;
    [SerializeField] private ItemQualityBackgroundConfig config;
    
    private Image _background;
    private IItemQualityBackgroundUser _user;

    private void Start()
    {
        _background = GetComponent<Image>();
        _user = GetComponentInParent<IItemQualityBackgroundUser>();
        _user.EventItemChanged += UpdateBackground;
        
        UpdateBackground();
    }

    private void UpdateBackground()
    {
        if (changeActiveState)
        {
            gameObject.SetActive(_user.Item != null);
        }

        _background.sprite = config.GetBackground(_user.Item);
    }
}
