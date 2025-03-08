using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TextAnimationBase : MonoBehaviour
{
    [Header("Animation Settings")]
    private float appearDuration = .8f;
    private float disappearDuration = 1f;
    private Tween tween;
    
    public void ExpandAlert()
    {
        if (tween != null) tween.Kill();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(Vector3.one * 1.3f, appearDuration)).SetEase(Ease.OutBack);
        sequence.Append(transform.DOScale(Vector3.one, appearDuration)).SetEase(Ease.OutExpo);
        sequence.WaitForCompletion();
        tween = sequence;
    }
}
