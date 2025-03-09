using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIAnimationBase : MonoBehaviour
{
    [Header("UI Settings")]
    public bool IsExpandedFirst;
    public bool DontHideOnStart;
    
    [Header("Animation Settings")]
    private float appearDuration = .5f;
    private float disappearDuration = .2f;
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
    
    public void Expand(Vector3 originalScale)
    {
        if (originalScale == default) originalScale = Vector3.one;
        if (tween != null) tween.Kill();
        transform.localScale = Vector3.zero;
        transform.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(originalScale * 1.2f, appearDuration)).SetEase(Ease.OutBack);
        sequence.Append(transform.DOScale(originalScale, appearDuration)).SetEase(Ease.OutBack);
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
    
    public void ExpandLikeTV()
    {
        if (tween != null) tween.Kill();
        transform.localScale = new Vector3(0, 0, 1);
        transform.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScaleX(1f, appearDuration)).SetEase(Ease.OutBack);
        sequence.Insert(.2f, transform.DOScaleY(1.2f, appearDuration)).SetEase(Ease.OutBack);
        sequence.Insert(.2f + appearDuration, transform.DOScaleY(1f, appearDuration)).SetEase(Ease.OutBack);
        sequence.WaitForCompletion();
        tween = sequence;
    }
    
    public void ShrinkLikeTV()
    {
        if (tween != null) tween.Kill();
    
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScaleY(0f, disappearDuration).SetEase(Ease.OutExpo))
            .OnComplete(() => transform.gameObject.SetActive(false));

        tween = sequence;
    }

}
