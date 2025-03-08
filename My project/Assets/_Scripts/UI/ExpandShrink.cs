using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ExpandShrink : MonoBehaviour
{
    private float speed = 1f;

    [SerializeField] private Vector3 originalScale;
    

    void OnEnable()
    {
        transform.DOScale(originalScale + Vector3.one * .1f, speed).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }
}
