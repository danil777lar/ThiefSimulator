using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class BlendedCharacterMovement : CharacterMovement
{ 
    protected const string _blendAnimationParameterName = "MoveBlend";
    protected int _blendAnimationParameter;
    
    protected override void InitializeAnimatorParameters()
    {
        base.InitializeAnimatorParameters();
        RegisterAnimatorParameter (_blendAnimationParameterName, AnimatorControllerParameterType.Float, out _blendAnimationParameter);
    }

    public override void UpdateAnimator()
    {
        base.UpdateAnimator();
        MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _blendAnimationParameter, 
            Mathf.Abs(_controller.CurrentMovement.magnitude) / MovementSpeed,
            _character._animatorParameters, _character.RunAnimatorSanityChecks);
    }
}
