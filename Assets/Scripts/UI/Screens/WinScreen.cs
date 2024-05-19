using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : UIScreen
{
    [Header("Buttons")]
    [SerializeField] private Button openButton;
    [SerializeField] private Button skipButton;

    [Header("Reward")] 
    [SerializeField] private GameObject bestRewardRoot;
    [SerializeField] private GameObject rewardItem;
    [SerializeField] private TextMeshProUGUI rewardCoins;
    
    [Header("UI")]
    [SerializeField] private RawImage caseImage;

    [Space]
    [SerializeField] private float nextStepDelay;
    [SerializeField] private List<LootStep> lootSteps;

    [InjectService] private ILevelManagerService _levelService;
    [InjectService] private UIService _uiService;

    private bool _bestRewardGiven;
    private int _currentStepIndex;
    
    protected override void OnBeforeOpen(UIObject.Args args)
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        
        openButton.onClick.AddListener(OnOpenButtonClicked);
        skipButton.onClick.AddListener(OnSkipButtonClicked);
        
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
    
    private void ShowCase()
    {
        LootStep lootStep = lootSteps[_currentStepIndex];
        lootStep.Case.gameObject.SetActive(true);
        
        RectTransform imageTransform = caseImage.rectTransform;
        caseImage.texture = lootStep.Case.Show((int)imageTransform.rect.width, (int)imageTransform.rect.height);
    }
    
    private void OnCaseShown()
    {
        openButton.gameObject.SetActive(true);
        bestRewardRoot.SetActive(true);
    }

    private void OnOpenButtonClicked()
    {
        lootSteps[_currentStepIndex].Case.Open();
        
        bestRewardRoot.SetActive(false);
        openButton.gameObject.SetActive(false);
    }
    
    private void OnCaseOpened()
    {
        rewardCoins.gameObject.SetActive(true);
        StartCoroutine(NextStepDelayCoroutine());
    }

    private void NextStep()
    {
        if (_currentStepIndex >= lootSteps.Count - 1 || _bestRewardGiven)
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
            .OpenScreen(new LoadingScreen.Args(false,null));
    }

    private IEnumerator NextStepDelayCoroutine()
    {
        yield return new WaitForSeconds(nextStepDelay);
        NextStep();
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
        [field: SerializeField] public float BestRewardChance { get; private set; }
    }
}
