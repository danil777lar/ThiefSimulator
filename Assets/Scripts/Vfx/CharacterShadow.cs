using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterShadow : MonoBehaviour
{
    [SerializeField] private HumanBodyBones attachToBone;

    private Character _character;
    private Transform _bone;

    private void Start()
    {
        _character = GetComponentInParent<Character>();
    }

    private void Update()
    {
        if (_character.CharacterAnimator != null && _bone == null)
        {
            _bone = _character.CharacterAnimator.GetBoneTransform(attachToBone);
        }

        if (_bone != null)
        {
            transform.position = _bone.position;
        }
    }
}
