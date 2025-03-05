using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIAnimationBase : MonoBehaviour
{
    [Header("Animation Settings")]
    private float appearDuration = .5f;
    private float disappearDuration = .6f;
    private Tween tween;
    
    public void Expand()
    {
        if (tween != null) tween.Kill();
        transform.localScale = Vector3.zero;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(Vector3.one * 1.2f, appearDuration)).SetEase(Ease.OutBack);
        sequence.Append(transform.DOScale(Vector3.one, appearDuration)).SetEase(Ease.OutBack);
        sequence.WaitForCompletion();
        tween = sequence;
    }
    
    public void Shrink()
    {
        if (tween != null) tween.Kill();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(Vector3.one * 1.2f, disappearDuration)).SetEase(Ease.InBack);
        sequence.Append(transform.DOScale(Vector3.zero, disappearDuration)).SetEase(Ease.InBack);
        sequence.WaitForCompletion();
        tween = sequence;
    }
}
