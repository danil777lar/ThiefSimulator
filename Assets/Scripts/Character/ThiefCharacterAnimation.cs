using Larje.Character;
using Larje.Character.Abilities;
using UnityEngine;

public class ThiefCharacterAnimation : MonoBehaviour
{
    private Animator _animator;
    private Character _character;
    private CharacterPhysics _physics;
    private CharacterWalk _characterWalk;
    private CharacterCarry3D _characterCarry;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _character = GetComponentInParent<Character>();
        _physics = _character.GetComponent<CharacterPhysics>();
        _characterWalk = _character.FindAbility<CharacterWalk>();
        _characterCarry = _character.FindAbility<CharacterCarry3D>();
    }

    private void Update()
    {
        _animator.SetBool("Carry", _characterCarry.HasCarryable); 
        _animator.SetBool("SpawnIn", false); 
        _animator.SetBool("SpawnOut", false); 

        _animator.SetFloat("SpeedPercent", _characterWalk.SpeedPercent); 
        _animator.SetFloat("ActualSpeed", _physics.HorizontalSpeed); 
    }
}
