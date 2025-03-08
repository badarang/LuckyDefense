using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossIcon : MonoBehaviour
{
    [SerializeField] private bool isAlert;
    [SerializeField] private Sprite normalIcon;
    [SerializeField] private Sprite alertIcon;
    [SerializeField] private Image currentIcon;

    public void ToggleBoss(bool isBoss)
    {
        if (isBoss && !isAlert)
        {
            isAlert = true;
            currentIcon.sprite = alertIcon;
        }
        else if (!isBoss && isAlert)
        {
            isAlert = false;
            currentIcon.sprite = normalIcon;
        }
    }

    private void Update()
    {
        if (isAlert)
        {
            float scaleFactor = 1.3f + Mathf.Sin(Time.time * 4) * 0.3f;
            transform.localScale = Vector3.one * scaleFactor;
        }
        else
        {
            float scaleFactor = 1f + Mathf.Sin(Time.time * 2) * 0.1f;
            transform.localScale = Vector3.one * scaleFactor;
        }
    }
}