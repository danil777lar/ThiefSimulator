using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class UpgradeInvisibility : UpgradeProcessor
{
    [Space(40f)]
    [SerializeField] private Material invisibilityMaterial;

    private Health _health;
    private CharacterVisionTarget _visionTarget;
    private MaterialSetter _materialSetter;

    public override void Init(int level)
    {
        base.Init(level);
        
        Character character = GetComponentInParent<Character>();
        
        _materialSetter = character.GetComponentInChildren<MaterialSetter>();
        _materialSetter.AddMaterialOverride(this, invisibilityMaterial);
        
        _visionTarget = character.GetComponentInChildren<CharacterVisionTarget>();
        _visionTarget.AddVisibilityMultiplier(VisibilityMultiplier);

        _health = character.CharacterHealth;
        _health.ImmuneToDamage = true;
    }

    public override void Remove()
    {
        base.Remove();
        
        _materialSetter.RemoveMaterialOverride(this);
        _visionTarget.RemoveVisibilityMultiplier(VisibilityMultiplier);
        _health.ImmuneToDamage = false;
    }
    
    private float VisibilityMultiplier()
    {
        return 0f;
    }
}
