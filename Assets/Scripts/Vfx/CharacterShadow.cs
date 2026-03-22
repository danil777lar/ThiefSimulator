using System.Collections;
using System.Collections.Generic;
using Larje.Character;
using UnityEngine;

public class CharacterShadow : MonoBehaviour
{
    [SerializeField] private HumanBodyBones attachToBone;

    private Character _character;
    private Animator _animator;
    private Transform _bone;

    private void Start()
    {
        _character = GetComponentInParent<Character>();
        _animator = _character.GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (_animator != null && _bone == null)
        {
            _bone = _animator.GetBoneTransform(attachToBone);
        }

        if (_bone != null)
        {
            transform.position = _bone.position;
        }
    }
}
