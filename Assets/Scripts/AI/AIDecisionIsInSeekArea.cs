using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class AIDecisionIsInSeekArea : AIDecision
{
    [SerializeField] private float distance = 2f;
    
    private CharacterSeek _seek;
    
    public override void Initialization()
    {
        base.Initialization();
        _seek = _brain.Owner.GetComponent<CharacterSeek>();
    }

    public override bool Decide()
    {
        return Vector3.Distance(_brain.Owner.transform.position, _seek.SeekPoint) <= distance;
    }
}
