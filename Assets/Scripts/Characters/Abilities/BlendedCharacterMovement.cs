using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class BlendedCharacterMovement : CharacterMovement
{ 
    protected const string _blendAnimationParameterName = "MoveBlend";
    protected int _blendAnimationParameter;

    public float CurrentSpeedPercent { get; private set; }

    protected override void InitializeAnimatorParameters()
    {
        base.InitializeAnimatorParameters();
        RegisterAnimatorParameter (_blendAnimationParameterName, AnimatorControllerParameterType.Float, out _blendAnimationParameter);
    }

    public override void ProcessAbility()
    {
        base.ProcessAbility();
        CurrentSpeedPercent = Mathf.Abs(_controller.CurrentMovement.magnitude) / MovementSpeed;
    }

    public override void UpdateAnimator()
    {
        base.UpdateAnimator();
        MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _blendAnimationParameter, CurrentSpeedPercent,
            _character._animatorParameters, _character.RunAnimatorSanityChecks);
    }
}
