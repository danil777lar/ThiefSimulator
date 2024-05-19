using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : UIScreen
{
    [SerializeField] private Button skipButton;
    [SerializeField] private RawImage caseImage;
    [SerializeField] private List<LootStep> lootSteps;

    [InjectService] private ILevelManagerService _levelService;
    [InjectService] private UIService _uiService;

    protected override void OnBeforeOpen(UIObject.Args args)
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        skipButton.onClick.AddListener(OnSkipButtonClicked);
        
        ShowCase();
    }

    private void OnSkipButtonClicked()
    {
        _levelService.IncrementLevelId();
        _levelService.SpawnCurrentLevel();
        _uiService.GetProcessor<UIScreenProcessor>()
            .OpenScreen(new LoadingScreen.Args(false,null));
    }

    private void ShowCase()
    {
        RectTransform imageTransform = caseImage.rectTransform;
        caseImage.texture = lootSteps[0].Case.GetTexture((int)imageTransform.rect.width, (int)imageTransform.rect.height);
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
