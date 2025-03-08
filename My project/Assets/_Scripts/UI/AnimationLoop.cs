using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimationLoop : MonoBehaviour
{
    private float speed = 1f;
    [SerializeField] private Vector2 originalPosition;
    [SerializeField] private Vector3 originalScale;
    [SerializeField] private int direction = 1;
    
    [Header("Animation Type")]
    [SerializeField] private bool isUpDown = false;
    [SerializeField] private bool isExpandShrink = false;
    [SerializeField] private bool isRotateY = false;
    

    void OnEnable()
    {
        if (isUpDown) transform.DOLocalMoveY(originalPosition.y + 0.25f * direction, speed).SetLoops(-1, LoopType.Yoyo);
        if (isExpandShrink) transform.DOScale(originalScale + Vector3.one * .1f, speed).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        if (isRotateY) transform.DORotate(new Vector3(0, 180, 0), speed).SetLoops(-1, LoopType.Incremental);
    }
}
