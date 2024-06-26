using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Services;
using ProjectConstants;
using UnityEngine;

public class UpgradeLockPicking : UpgradeProcessor
{
    private List<IMiniGameLauncher> _lockPickMiniGames = new List<IMiniGameLauncher>();
    
    public override void Init(int level)
    {
        base.Init(level);
        
        LevelProcessor levelRoot = GetComponentInParent<LevelProcessor>();
        _lockPickMiniGames = levelRoot.GetComponentsInChildren<IMiniGameLauncher>()
            .ToList().FindAll(x => x.MiniGameType == MiniGameType.LockMiniGame);
        _lockPickMiniGames.ForEach(x => x.AddMultiplier(GetMultiplier));
    }

    public override void Remove()
    {
        base.Remove();
        _lockPickMiniGames.ForEach(x => x.RemoveMultiplier(GetMultiplier));
    }

    private float GetMultiplier()
    {
        return 1f + GetValueOnLevel(_currentLevel);
    }
}
