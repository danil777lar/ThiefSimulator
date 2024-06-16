using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class UpgradeInvisibility : UpgradeProcessor
{
    [Space(40f)]
    [SerializeField] private Material invisibilityMaterial;
    
    private MaterialSetter _materialSetter;
    
    public override void Init(int level)
    {
        base.Init(level);
        
        _materialSetter = GetComponentInParent<MaterialSetter>();
        _materialSetter.AddMaterialOverride(this, invisibilityMaterial);
    }

    public override void Remove()
    {
        base.Remove();
        
        _materialSetter.RemoveMaterialOverride(this);
    }
}
