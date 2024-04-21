using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class AnimatorParameterSetter : MonoBehaviour
{
    [SerializeField] private float stealthValue;
    
    private void Start()
    {
        Character character = GetComponentInParent<Character>();
        character.CharacterAnimator.SetFloat("Stealth", stealthValue);        
    }
}
