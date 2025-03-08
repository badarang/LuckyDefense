using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIAnimationBase : MonoBehaviour
{
    [Header("UI Settings")]
    public bool IsExpandedFirst;
    
    [Header("Animation Settings")]
    private float appearDuration = .5f;
    private float disappearDuration = .1f;
    private Tween tween;
    
    public void Expand()
    {
        if (tween != null) tween.Kill();
        transform.localScale = Vector3.zero;
        transform.gameObject.SetActive(true);
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
        sequence.Append(transform.DOScale(Vector3.zero, disappearDuration).SetEase(Ease.OutExpo))
            .OnComplete(() => transform.gameObject.SetActive(false));

        tween = sequence;
    }

}
