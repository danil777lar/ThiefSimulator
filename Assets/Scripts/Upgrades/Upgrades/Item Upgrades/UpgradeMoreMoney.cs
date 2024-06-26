using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Services;
using UnityEngine;

public class UpgradeMoreMoney : UpgradeProcessor
{
    private List<SellPoint> _sellPoints;
    
    public override void Init(int level)
    {
        base.Init(level);

        LevelProcessor levelRoot = GetComponentInParent<LevelProcessor>();
        _sellPoints = levelRoot.GetComponentsInChildren<SellPoint>().ToList();
        _sellPoints.ForEach(x => x.AddSellPriceMultiplier(GetMultiplier));
    }

    public override void Remove()
    {
        base.Remove();
        
        _sellPoints.ForEach(x => x.RemoveSellPriceMultiplier(GetMultiplier));
    }

    private float GetMultiplier()
    {
        return 1f + GetValueOnLevel(_currentLevel);
    }
}
