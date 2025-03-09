using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextAnimationBase : MonoBehaviour
{
    [Header("Animation Settings")]
    private float appearDuration = .8f;
    private float disappearDuration = 1f;
    private Tween tween;
    
    public void ExpandAlertDelayed(float delay)
    {
        StartCoroutine(ExpandAlertCoroutine(delay));
    }
    
    IEnumerator ExpandAlertCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        ExpandAlert();
    }
    
    public void ExpandAlert()
    {
        if (tween != null) tween.Kill();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(Vector3.one * 1.3f, appearDuration)).SetEase(Ease.OutBack);
        sequence.Append(transform.DOScale(Vector3.one, appearDuration)).SetEase(Ease.OutExpo);
        sequence.WaitForCompletion();
        tween = sequence;
    }
    
    public void ExpandAlert(Color color)
    {
        var originalColor = Color.white;
        if (tween != null) tween.Kill();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(Vector3.one * 1.3f, appearDuration)).SetEase(Ease.OutBack);
        sequence.Insert(0f, transform.GetComponent<TextMeshProUGUI>().DOColor(color, appearDuration)).SetEase(Ease.OutExpo);
        sequence.Append(transform.DOScale(Vector3.one, appearDuration)).SetEase(Ease.OutExpo);
        sequence.Insert(appearDuration, transform.GetComponent<TextMeshProUGUI>().DOColor(originalColor, appearDuration)).SetEase(Ease.OutExpo);
        sequence.WaitForCompletion();
        tween = sequence;
    }
}
