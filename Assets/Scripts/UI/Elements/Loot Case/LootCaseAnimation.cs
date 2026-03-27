using System;
using UnityEngine;

public abstract class LootCaseAnimation : MonoBehaviour
{
    public abstract void PlayShowAnimation(Action onAnimationComplete);
    public abstract void PlayOpenAnimation(Action onAnimationComplete);
}
