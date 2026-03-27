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
    [SerializeField] private float levelStartCutsceneDuration = 2f;

    [InjectService] private UIService _uiService;
    [InjectService] private GameEventService _gameEventService;
    [InjectService] private IGameStateService _gameStateService;
    [InjectService] private ILevelManagerService _levelManagerService;

    public override void Init()
    {
        _gameStateService.EventGameStateChanged += OnOnGameStateChanged;

        StartGame();
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
        Load(() => 
        {
            StartLevel();
        });
    }

    public void ReturnToMenu()
    {
        StartGame();
    }

    private void OnOnGameStateChanged(GameState oldState, GameState newState)
    {
        if (newState == GameStates.Win)
        {
            _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(new WinScreen.Args());
        }
    }

    private void StartGame()
    {
        _levelManagerService.SpawnCurrentLevel();
        Load(() => 
        {
            MenuScreen.Args menuScreenArgs = new MenuScreen.Args();
            _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(menuScreenArgs);
            _gameStateService.SetGameState(GameStates.Menu);
        });
    }

    private void Load(Action onComplete)
    {
        LoadingScreen.Args loadingScreen = new LoadingScreen.Args(true, () => onComplete?.Invoke());

        _gameStateService.SetGameState(GameStates.Loading);
        _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(loadingScreen);
    }
}
