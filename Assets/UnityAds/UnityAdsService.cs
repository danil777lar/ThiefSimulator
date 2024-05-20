using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Serialization;

[BindService(typeof(IAdsService))]
public class UnityAdsService : Service, IAdsService, IUnityAdsInitializationListener
{
    [Header("System")]
    [SerializeField] private bool testMode = true;
    [SerializeField] private bool logsEnabled = true;
    [Header("Keys")]
    [SerializeField] private Keys androidKeys;
    [SerializeField] private Keys iosKeys;
    [Header("Banner")] 
    [SerializeField] private bool useBanner;
    [SerializeField] private BannerPosition bannerPosition = BannerPosition.BOTTOM_CENTER;
    
    private Keys _keys;
    
    private UnityAdsUniversal _interstitial;
    private UnityAdsUniversal _rewarded;
    private UnityAdsBanner _banner;
 
    public override void Init()
    {
        #if UNITY_IOS
            _keys = iosKeys;
        #elif UNITY_ANDROID
            _keys = androidKeys;
        #elif UNITY_EDITOR
            _keys = androidKeys;
        #endif
        
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            _interstitial = new UnityAdsUniversal(_keys.InterstitialId, logsEnabled);
            _rewarded = new UnityAdsUniversal(_keys.RewardedId, logsEnabled);
            _banner = new UnityAdsBanner(_keys.RewardedId, bannerPosition, logsEnabled);
            
            Advertisement.Initialize(_keys.GameId, testMode, this);
        }   
    }
 
    public void OnInitializationComplete()
    {
        _interstitial.LoadAd();
        _rewarded.LoadAd();
        _banner.LoadBanner();
        
        Debug.Log("Unity Ads initialization complete.");
    }
 
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    [Serializable]
    private class Keys
    {
        [field: SerializeField] public string GameId { get; private set; }
        [Header("Ads")] 
        [field: SerializeField] public string InterstitialId;
        [FormerlySerializedAs("RewardId")] [field: SerializeField] public string RewardedId;
        [field: SerializeField] public string BannerId;
    }
}
