using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Character;
using Larje.Character.AI;
using UnityEngine;
using UnityEngine.AI;

public class AIActionAimToPlayer : AIAction
{ 
    private Transform _target;
    private Character _player;

    protected override void OnInitialized()
    {
        base.Initialization();
        
        _player = FindPlayer();
        
        _target = new GameObject().transform;
        _target.gameObject.name = "Aim To Player Target";
        _target.SetParent(transform);
    }

    protected override void OnEnterState()
    {
        base.OnEnterState();
        Brain.Target = _target;
    }

    public override void PerformAction()
    {
        if (_player)
        {
            _target.position = _player.transform.position;
        }
    }

    private Character FindPlayer()
    {
        // FindObjectsByType<Character>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList().Find(x => x.CharacterType == Character.CharacterTypes.Player);
        return null;
    }
}
