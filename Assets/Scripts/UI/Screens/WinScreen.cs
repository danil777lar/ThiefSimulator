using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using Larje.Core.Tools;
using ProjectConstants;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WinScreen : UIScreen, IItemQualityBackgroundUser
{
    [SerializeField] private float showSkipButtonDelay = 2f;
    
    [Header("Buttons")]
    [SerializeField] private Button openButton;
    [SerializeField] private Button skipButton;

    [Header("Reward")] 
    [SerializeField] private GameObject bestRewardRoot;
    [SerializeField] private TextMeshProUGUI bestRewardChance;
    [Space]
    [SerializeField] private Image bestRewardIcon;
    [SerializeField] private Image rewardItemIcon;
    [Space]
    [SerializeField] private GameObject rewardItem;
    [SerializeField] private TextMeshProUGUI rewardCoins;
    
    [Header("UI")]
    [SerializeField] private RawImage caseImage;
    [SerializeField] private GameObject openButtonAdIcon;

    [Space]
    [SerializeField] private float nextStepDelay;
    [SerializeField] private List<LootStep> lootSteps;

    [InjectService] private UIService _uiService;
    [InjectService] private ILevelManagerService _levelService;
    [InjectService] private ICurrencyService _currencyService;
    [InjectService] private IAdsService _adsService;
    [InjectService] private ItemHolderService _itemsService;
    [InjectService] private IDataService _dataService;

    private bool _bestRewardGiven;
    private bool _rewardedShown;
    private int _currentStepIndex;
    private Item _bestReward;
    private ItemType _bestRewardType;
    private RewardedAdButton _rewardAdButton;
    private IEnumerator _showSkipButtonCoroutine;
    
    public ThiefItem Item => _bestReward as ThiefItem;
    public event Action EventItemChanged;
    
    protected override void OnBeforeOpen(UIObject.Args args)
    {
        DIContainer.InjectTo(this);
        
        GrabBestReward();
        
        _currencyService.MoveCurrency(CurrencyType.Coins, CurrencyPlacementType.Level, CurrencyPlacementType.Global);
        
        openButton.onClick.AddListener(OnOpenButtonClicked);
        skipButton.onClick.AddListener(OnSkipButtonClicked);

        _rewardAdButton = openButton.GetComponent<RewardedAdButton>();
        
        bestRewardRoot.SetActive(false);
        openButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
        
        lootSteps.ForEach(x =>
        {
            x.Case.EventShown += OnCaseShown;
            x.Case.EventOpened += OnCaseOpened;
            x.Case.gameObject.SetActive(false);
        });
        
        ShowCase();
    }

    private void GrabBestReward()
    {
        Dictionary<Item, ItemType> lockedItems = new Dictionary<Item, ItemType>();
        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            foreach (Item item in _itemsService.GetLockedItems(type))
            {
                lockedItems.Add(item, type);
            }
        }

        if (lockedItems.Count > 0)
        {
            _bestReward = lockedItems.Keys.ToList()[Random.Range(0, lockedItems.Count)];
            _bestRewardType = lockedItems[_bestReward];

            bestRewardIcon.sprite = _bestReward.Icon;
            rewardItemIcon.sprite = _bestReward.Icon;
            
            EventItemChanged?.Invoke();
        }
    }
    
    private void ShowCase()
    {
        LootStep lootStep = lootSteps[_currentStepIndex];
        lootStep.Case.gameObject.SetActive(true);
        
        bestRewardChance.text = $"CHANCE: {lootStep.BestRewardChance}%";
        
        RectTransform imageTransform = caseImage.rectTransform;
        caseImage.texture = lootStep.Case.Show((int)imageTransform.rect.width, (int)imageTransform.rect.height);
    }
    
    private void OnCaseShown()
    {
        openButton.gameObject.SetActive(true);
        _rewardAdButton.SetEnableChecking(lootSteps[_currentStepIndex].WithAd);
        openButtonAdIcon.gameObject.SetActive(lootSteps[_currentStepIndex].WithAd);

        if (lootSteps[_currentStepIndex].WithAd)
        {
            _showSkipButtonCoroutine = ShowSkipButtonDelayCoroutine();
            StartCoroutine(_showSkipButtonCoroutine);
        }

        if (_bestReward != null)
        {
            bestRewardRoot.SetActive(true);
        }
    }

    private void OnOpenButtonClicked()
    {
        if (lootSteps[_currentStepIndex].WithAd)
        {
            _rewardedShown = true;
            _adsService.ShowRewarded(null, null, OpenCase, null);
        }
        else
        {
            OpenCase();
        }
    }

    private void OpenCase()
    {
        if (_showSkipButtonCoroutine != null)
        {
            StopCoroutine(_showSkipButtonCoroutine);
        }
        
        lootSteps[_currentStepIndex].Case.Open();
        bestRewardRoot.SetActive(false);
        openButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
    }
    
    private void OnCaseOpened()
    {
        GiveReward();
        StartCoroutine(NextStepDelayCoroutine());
    }
    
    private void GiveReward()
    {
        if (_bestReward != null && Random.Range(0, 100) < lootSteps[_currentStepIndex].BestRewardChance)
        {
            _bestRewardGiven = true;
            _itemsService.UnlockItem(_bestRewardType, _bestReward.Name);
            _itemsService.SetCurrentItem(_bestRewardType, _bestReward.Name);
            
            rewardItem.SetActive(true);
        }
        else
        {
            int coinsAmount = Random.Range(lootSteps[_currentStepIndex].CoinsMin, lootSteps[_currentStepIndex].CoinsMax);
            rewardCoins.text = $"${coinsAmount}";
            rewardCoins.gameObject.SetActive(true);
            
            _currencyService.AddCurrency(CurrencyType.Coins, CurrencyPlacementType.Global, coinsAmount);
            _dataService.SaveGameData();
        }
    }

    private void NextStep()
    {
        if (_currentStepIndex >= lootSteps.Count - 1 || _bestRewardGiven || _bestReward == null)
        {
            CloseScreen();
        }
        else
        {
            lootSteps[_currentStepIndex].Case.gameObject.SetActive(false);
            rewardCoins.gameObject.SetActive(false);
            rewardItem.gameObject.SetActive(false);
            bestRewardRoot.SetActive(false);
            openButton.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(false);
            
            _currentStepIndex++;
            ShowCase();
        }
    }

    private void OnSkipButtonClicked()
    {
        CloseScreen();
    }

    private void CloseScreen()
    {
        _levelService.IncrementLevelId();
        _levelService.SpawnCurrentLevel();
        _uiService.GetProcessor<UIScreenProcessor>()
            .OpenScreen(new LoadingScreen.Args(!_rewardedShown,null));
    }

    private IEnumerator NextStepDelayCoroutine()
    {
        yield return new WaitForSeconds(nextStepDelay);
        NextStep();
    }
    
    private IEnumerator ShowSkipButtonDelayCoroutine()
    {
        yield return new WaitForSeconds(showSkipButtonDelay);
        skipButton.gameObject.SetActive(true);
    }

    public class Args : UIScreen.Args
    {
        public Args() : base(UIScreenType.Win)
        {
            
        }
    }

    [Serializable]
    private class LootStep
    {
        [field: SerializeField] public LootCase Case { get; private set; }
        [field: SerializeField] public bool WithAd { get; private set; }
        [field: SerializeField, Max(100)] public int BestRewardChance { get; private set; }
        [field: SerializeField] public int CoinsMin { get; private set; }
        [field: SerializeField] public int CoinsMax { get; private set; }
    }
}
