using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using ProjectConstants;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayScreen : UIScreen
{
    [Header("Buttons")]
    [SerializeField] private Button pauseButton;
    [Header("Action Buttons")]
    [SerializeField] private PlayerActionButton actionButtonPrefab; 
    [SerializeField] private RectTransform actionButtonsRoot;
    [Header("Progress")] 
    [SerializeField] private Slider sliderTotal; 
    [SerializeField] private Slider sliderWin; 
    [SerializeField] private Slider sliderWeight; 
    
    [InjectService] private ILevelManagerService _levelService;
    [InjectService] private UIService _uiService;
    [InjectService] private PlayerInputService _inputService;

    private Character _player;
    private CharacterCarry3D _playerCarry;
    private ThiefLevel.LevelData _levelData;
    private List<PlayerActionButton> _actionButtons;

    protected override void OnBeforeOpen(UIObject.Args args)
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        
        pauseButton.onClick.AddListener(OnPauseButtonClicked);
        
        sliderTotal.value = 0f;
        sliderWin.value = 0f;
        
        _levelData = _levelService.GetCurrentLevelData<ThiefLevel.LevelData>();
        _inputService.ConnectPlayer();
        
        _player = FindObjectsByType<Character>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList()
            .Find(x => x.CharacterType == Character.CharacterTypes.Player);
        _playerCarry = _player.FindAbility<CharacterCarry3D>();
        
        if (_player != null)
        {
            actionButtonsRoot.MMDestroyAllChildren();
            _actionButtons = new List<PlayerActionButton>();
            IPlayerActionSource[] actionSources = _player.GetComponents<IPlayerActionSource>();
            foreach (IPlayerActionSource source in actionSources)
            {
                foreach (PlayerAction playerAction in source.Actions)
                {
                    _actionButtons.Add(Instantiate(actionButtonPrefab, actionButtonsRoot).Build(playerAction));
                }   
            }
        }
    }

    protected override bool OnBack(bool onlyOverride)
    {
        _uiService.GetProcessor<UIPopupProcessor>().OpenPopup(new PausePopup.Args());
        return true;
    }

    private void Update()
    {
        UpdateActionButtons();
        UpdateProgress();
        UpdateCarryWeight();
    }

    private void UpdateActionButtons()
    {
        bool blockUpdate = false;
        _actionButtons.ForEach(x => blockUpdate |= x.PointerDown);
        _actionButtons.ForEach(x => x.blockUpdateUI = blockUpdate);
    }

    private void UpdateProgress()
    {
        sliderTotal.value = Mathf.Lerp(sliderTotal.value, _levelData.ProgressTotal, Time.deltaTime * 5f);
        sliderWin.value = Mathf.Lerp(sliderWin.value, _levelData.ProgressForWin, Time.deltaTime * 5f);
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
