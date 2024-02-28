using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionButton : MonoBehaviour
{
    private PlayerAction _action;
    
    public PlayerActionButton Build(PlayerAction action)
    {
        _action = action;
        return this;
    }
}
