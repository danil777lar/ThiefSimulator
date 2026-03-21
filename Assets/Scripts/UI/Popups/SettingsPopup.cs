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
    
    [InjectService] private DataService _dataService;
    
    protected override void OnBeforeOpen(UIObject.Args args)
    {
        DIContainer.InjectTo(this);

        // soundToggle.isOn = _dataService.Data.Settings.SoundGlobal;
        // vibrationToggle.isOn = _dataService.Data.Settings.Vibration;
        
        // soundToggle.onValueChanged.AddListener(OnSoundToggleValueChanged);
        // vibrationToggle.onValueChanged.AddListener(OnVibrationToggleValueChanged);
        privacyPolicyButton.onClick.AddListener(OnPrivacyPolicyButtonClicked);
        closeButton.onClick.AddListener(Close);
    }

    private void OnSoundToggleValueChanged(bool value)
    {
        // _dataService.Data.Settings.SoundGlobal = value;
        // _dataService.Save();
    }
    
    private void OnVibrationToggleValueChanged(bool value)
    {
        // _dataService.Data.Settings.Vibration = value;
        // _dataService.Save();
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
