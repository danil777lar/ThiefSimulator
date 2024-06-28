using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;

public class ToastShower : MonoBehaviour
{
    [SerializeField] private string toastText;
    [SerializeField] private UIToastType toastType;

    [InjectService] private UIService _uiService;
    
    private void Start()
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
    }
    
    public void ShowToast()
    {
        UIToast.Args toast = new UIToast.Args(toastType, toastText);
        _uiService.GetProcessor<UIToastProcessor>().OpenToast(toast);
    }
}
