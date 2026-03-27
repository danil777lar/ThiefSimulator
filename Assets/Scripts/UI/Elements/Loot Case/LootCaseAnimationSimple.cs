using System;
using DG.Tweening;
using UnityEngine;

public class LootCaseAnimationSimple : LootCaseAnimation
{
    [SerializeField] private Transform root;
    [Space]
    [SerializeField] private Transform[] lockParts;
    [SerializeField] private Transform topPart;

    [ContextMenu("Play Show Animation")]
    public void PlayShowAnimation()
    {
        PlayShowAnimation(null);
    }

    public override void PlayShowAnimation(Action onAnimationComplete)
    {
        Sequence sequence = DOTween.Sequence();

        root.localScale = Vector3.zero;
        sequence.Append(root.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBack));

        sequence.Join(root.DORotate(new Vector3(0f, 360f * 2f, 0f), 2f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.OutElastic));

        sequence.OnComplete(() => onAnimationComplete?.Invoke());
    }

    [ContextMenu("Play Open Animation")]
    public void PlayOpenAnimation()
    {
        PlayOpenAnimation(null);
    }

    public override void PlayOpenAnimation(Action onAnimationComplete)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(root.DOScale(new Vector3(0.5f, 1.5f, 0.5f), 1f)
            .SetEase(Ease.OutQuart));

        sequence.Append(topPart.DOLocalRotate(Vector3.right * -120f, 1f).SetEase(Ease.OutElastic));
        sequence.Join(root.DOScale(Vector3.one, 1f).SetEase(Ease.OutElastic));
        foreach (Transform lockPart in lockParts)
        {
            sequence.Join(lockPart.DOLocalRotate(Vector3.right * 140f, 1f).SetEase(Ease.OutElastic));
        }


        sequence.OnComplete(() => onAnimationComplete?.Invoke());
    }
}
