using Larje.Character;
using Larje.Core.Services;
using ProjectConstants;
using UnityEngine;

public class PlayerRevive : MonoBehaviour, ILevelStartHandler
{
    private Character _character;

    public void OnLevelStarted(LevelProcessor.StartData data)
    {
        if (data.StartType == LevelStartType.Revive)
        {
            _character.Health.Revive();
        }
    }

    private void Start()
    {
        _character = GetComponentInParent<Character>();
    }
}
