using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Character;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayScreen : UIScreen
{
    [Header("Buttons")]
    [SerializeField] private Button pauseButton;

    [Header("Progress")] 
    [SerializeField] private Slider sliderTotal; 
    [SerializeField] private Slider sliderWeight; 
    
    [InjectService] private UIService _uiService;
    [InjectService] private PlayerInputService _inputService;
    [InjectService] private ILevelManagerService _levelService;
    [InjectService] private IPlayerProviderService _playerProviderService;

    private Character _player;
    private CharacterCarry3D _playerCarry;
    private ThiefLevel.LevelData _levelData;

    protected override void OnBeforeOpen(UIObject.Args args)
    {
        DIContainer.InjectTo(this);
        
        pauseButton.onClick.AddListener(OnPauseButtonClicked);
        sliderTotal.value = 0f;

        _levelData = _levelService.GetCurrentLevelData<ThiefLevel.LevelData>();
        _playerProviderService.TryGetPlayer(out _player);
        _playerCarry = _player.FindAbility<CharacterCarry3D>();
    }

    protected override bool OnBack(bool onlyOverride)
    {
        _uiService.GetProcessor<UIPopupProcessor>().OpenPopup(new PausePopup.Args());
        return true;
    }

    private void Update()
    {
        UpdateProgress();
        UpdateCarryWeight();
    }

    private void UpdateProgress()
    {
        sliderTotal.value = Mathf.Lerp(sliderTotal.value, _levelData.Progress, Time.deltaTime * 5f);
    }

    private void UpdateCarryWeight()
    {
        float value = _playerCarry.CurrentWeight / _playerCarry.WeightCapacity;
        sliderWeight.value = Mathf.Lerp(sliderWeight.value, value, Time.deltaTime * 5f);
    }

    private void OnPauseButtonClicked()
    {
        _uiService.GetProcessor<UIPopupProcessor>().OpenPopup(new PausePopup.Args());
    }

    public class Args : UIScreen.Args
    {
        public Args() : base(UIScreenType.Play)
        {
            
        }
    }
}
