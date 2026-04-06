using System;
using DG.Tweening;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;

[BindService(typeof(ThiefGameService))]
public class ThiefGameService : Service
{
    [Header("Loading")]
    [SerializeField] private float firstStartLoadingDuration;
    [SerializeField] private float usualLoadingDuration;
    [SerializeField] private float levelLoadingDuration;

    [Space]
    [SerializeField] private float levelStartCutsceneDuration = 2f;


    [InjectService] private UIService _uiService;
    [InjectService] private GameEventService _gameEventService;
    [InjectService] private IDataService _dataService;
    [InjectService] private IGameStateService _gameStateService;
    [InjectService] private ILevelManagerService _levelManagerService;
    [InjectService] private IAdsService _adsService;

    public override void Init()
    {
        _gameStateService.EventGameStateChanged += OnOnGameStateChanged;

        bool firstStart = _dataService.SystemData.IternalData.SessionNum <= 1;
        LoadCurrentLevel(firstStart ? firstStartLoadingDuration : usualLoadingDuration, !firstStart);
    }

    public void StartLevel()
    {
        if (_gameStateService.CurrentState == GameStates.Cutscene || _gameStateService.CurrentState == GameStates.Playing)
        {
            return;
        }

        _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(new PlayScreen.Args());
        _gameStateService.SetGameState(GameStates.Cutscene);
        _gameEventService.SendEvent(new LevelEventPreStart(levelStartCutsceneDuration));

        DOVirtual.DelayedCall(levelStartCutsceneDuration, () =>
        {
            _gameStateService.SetGameState(GameStates.Playing);
            _levelManagerService.TryStartCurrentLevel(new LevelProcessor.StartData(LevelStartType.Start));
        });
    }

    public void RestartLevel()
    {
        _levelManagerService.SpawnCurrentLevel();
        Load(levelLoadingDuration, true, StartLevel);
    }

    public void ReturnToMenu()
    {
        LoadCurrentLevel(levelLoadingDuration, true);
    }

    private void OnOnGameStateChanged(GameState oldState, GameState newState)
    {
        if (newState == GameStates.Win)
        {
            _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(new WinScreen.Args((rewardedShown) => 
            {
                _levelManagerService.IncrementLevelId();
                LoadCurrentLevel(levelLoadingDuration, !rewardedShown);
            }));
        }

        if (newState == GameStates.Fail)
        {
            _uiService.GetProcessor<UIPopupProcessor>().OpenPopup(new RevivePopup.Args(OnPlayerRevive, OnPlayerFail));
        }
    }

    private void LoadCurrentLevel(float duration, bool showInter)
    {
        _levelManagerService.SpawnCurrentLevel();

        Load(duration, showInter, () => 
        {
            MenuScreen.Args menuScreenArgs = new MenuScreen.Args();
            _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(menuScreenArgs);
            _gameStateService.SetGameState(GameStates.Menu);
        });
    }

    private void Load(float duration, bool showInter, Action onComplete)
    {
        if (_gameStateService.CurrentState == GameStates.Loading)
        {
            return;
        }
        _gameStateService.SetGameState(GameStates.Loading);

        float loadingProgress = 0f;
        DOVirtual.Float(0f, 1f, duration, value => loadingProgress = value)
            .OnComplete(() => 
            {
                onComplete?.Invoke();
                if (showInter)
                {
                    _adsService.ShowInterstitial(0);
                }
            });

        LoadingScreen.Args loadingScreen = new LoadingScreen.Args(() => loadingProgress);
        _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(loadingScreen);
    }

    private void OnPlayerRevive()
    {
        _levelManagerService.TryStartCurrentLevel(new LevelProcessor.StartData(LevelStartType.Revive));
    }

    private void OnPlayerFail()
    {
        _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(new FailScreen.Args(() => LoadCurrentLevel(levelLoadingDuration, true)));
    }
}
