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
    }

    public void StartLevel()
    {
        _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(new PlayScreen.Args());
        _gameStateService.SetGameState(GameStates.Cutscene);
        _gameEventService.SendEvent(new LevelEventPreStart(levelStartCutsceneDuration));

        DOVirtual.DelayedCall(levelStartCutsceneDuration, () =>
        {
            _gameStateService.SetGameState(GameStates.Playing);
            _levelManagerService.TryStartCurrentLevel(new LevelProcessor.StartData(LevelStartType.Start));
        });
    }
}
