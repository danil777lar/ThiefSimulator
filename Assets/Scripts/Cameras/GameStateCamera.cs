using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using Unity.Cinemachine;
using UnityEngine;

public class GameStateCamera : MonoBehaviour
{
    [SerializeField] private List<GameState> activeInStates;
    [SerializeField] private int priority;

    [InjectService] private IGameStateService _gameStateService;

    private CinemachineCamera _cam;

    private bool IsActiveNow => activeInStates.Contains(_gameStateService.CurrentState);

    private void Start()
    {
        DIContainer.InjectTo(this);

        _cam = GetComponentInChildren<CinemachineCamera>();
        _cam.Priority = IsActiveNow ? priority : 0;

        _gameStateService.EventGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        _gameStateService.EventGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState oldState, GameState newState)
    {
        _cam.Priority = IsActiveNow ? priority : 0;
    }
}
