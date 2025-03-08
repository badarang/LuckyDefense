using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveUpDown : MonoBehaviour
{
    private float speed = 1f;
    [SerializeField] private Vector2 originalPosition;
    [SerializeField] private int direction = 1;
    

    void OnEnable()
    {
        transform.DOLocalMoveY(originalPosition.y + 0.25f * direction, speed).SetLoops(-1, LoopType.Yoyo);
    }
}
