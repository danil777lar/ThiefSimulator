using System.Collections;
using System.Collections.Generic;
using Larje.Character;
using UnityEngine;

public class AnimatorParameterSetter : MonoBehaviour
{
    [SerializeField] private float stealthValue;
    
    private void Start()
    {
        Character character = GetComponentInParent<Character>();
        Animator animator = character.GetComponentInChildren<Animator>();
        animator.SetFloat("Stealth", stealthValue);        
    }
}
