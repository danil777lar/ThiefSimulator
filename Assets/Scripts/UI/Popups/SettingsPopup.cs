using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : UIPopup
{
    [SerializeField] private string privacyPolicyUrl = "https://www.google.com";
    [Space]
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle vibrationToggle;
    [SerializeField] private Button privacyPolicyButton;
    [SerializeField] private Button closeButton;
    
    [InjectService] private IDataService _dataService;
    
    protected override void OnBeforeOpen(UIObject.Args args)
    {
        DIContainer.InjectTo(this);
        
        soundToggle.isOn = _dataService.SystemData.Settings.SoundData.GetChannel("Default").Volume > 0f;
        soundToggle.onValueChanged.AddListener(OnSoundToggleValueChanged);

        vibrationToggle.isOn = _dataService.SystemData.Settings.VibrationGlobal;
        vibrationToggle.onValueChanged.AddListener(OnVibrationToggleValueChanged);

        privacyPolicyButton.onClick.AddListener(OnPrivacyPolicyButtonClicked);
        closeButton.onClick.AddListener(Close);
    }

    private void OnSoundToggleValueChanged(bool value)
    {
        _dataService.SystemData.Settings.SoundData.GetChannel("Default").Volume = value ? 1f : 0f;
        _dataService.SaveSystemData();
    }
    
    private void OnVibrationToggleValueChanged(bool value)
    {
        _dataService.SystemData.Settings.VibrationGlobal = value;
        _dataService.SaveSystemData();
    }
    
    private void OnPrivacyPolicyButtonClicked()
    {
        Application.OpenURL(privacyPolicyUrl);
    }


    public new class Args : UIPopup.Args
    {
        public Args() : base(UIPopupType.Settings)
        {
        }

        public Args(UIPopupCombinationType combinationType) : base(UIPopupType.Settings, combinationType)
        {
        }
    }
}
