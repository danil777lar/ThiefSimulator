using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class AIActionAimToSeekPoint : AIAction
{
    private Transform _target;
    private CharacterSeek _seek;
    
    public override void Initialization()
    {
        base.Initialization();
        _seek = _brain.Owner.GetComponent<CharacterSeek>();

        _target = new GameObject().transform;
        _target.gameObject.name = "Run To Seek Point Target";
        _target.SetParent(transform);
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        _brain.Target = _target;
    }

    public override void PerformAction()
    {
        _target.position = _seek.SeekPoint;
    }
}
