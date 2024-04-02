using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.AI;

public class AIActionAimToPlayer : AIAction
{ 
    private Transform _target;
    private Character _player;

    public override void Initialization()
    {
        base.Initialization();
        
        _player = FindObjectsByType<Character>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .ToList().Find(x => x.CharacterType == Character.CharacterTypes.Player);
        
        _target = new GameObject().transform;
        _target.gameObject.name = "Aim To Player Target";
        _target.SetParent(transform);
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        _brain.Target = _target;
    }

    public override void PerformAction()
    {
        if (_player)
        {
            _target.position = _player.transform.position;
        }
    }
}
