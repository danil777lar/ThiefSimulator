using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : UIPopup
{
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle vibrationToggle;
    [SerializeField] private Button privacyPolicyButton;
    [SerializeField] private Button closeButton;
    
    protected override void OnBeforeOpen(UIObject.Args args)
    {
        soundToggle.onValueChanged.AddListener(OnSoundToggleValueChanged);
        vibrationToggle.onValueChanged.AddListener(OnVibrationToggleValueChanged);
        privacyPolicyButton.onClick.AddListener(OnPrivacyPolicyButtonClicked);
        closeButton.onClick.AddListener(Close);
    }

    private void OnSoundToggleValueChanged(bool value)
    {
        
    }
    
    private void OnVibrationToggleValueChanged(bool value)
    {
        
    }
    
    private void OnPrivacyPolicyButtonClicked()
    {
        
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
