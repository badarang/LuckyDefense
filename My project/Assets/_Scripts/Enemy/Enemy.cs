using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public abstract class Enemy : MonoBehaviour, IHittable
{
    private EntityAnimator _entityAnimator;
    [SerializeField] private float hp;
    [SerializeField] private float speed;
    public float Speed => speed;
    [SerializeField] private int dropGold;
    public bool IsDead { get; set; }

    public void Init()
    {
        _entityAnimator = GetComponentInChildren<EntityAnimator>();
        _entityAnimator.Init();
        _entityAnimator.InitEnemy(this);
        IsDead = false;
        RoundManager.Instance.AliveEnemies++;
        Appear();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        DOTween.Kill(_entityAnimator);
    }
    
    private void Appear()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack);
    }

    public void TakeDamage(float amount, bool isAttackerRight = true)
    {
        hp -= amount;
        _entityAnimator.StartHitAnimation(isAttackerRight);
        if (hp <= 0)
        {
            Die();
        }
    }
    
    public void Die()
    {
        if (IsDead) return;
        IsDead = true;
        GoodsManager.Instance.Gold += dropGold;
        RoundManager.Instance.AliveEnemies--;
        StartCoroutine(DestroyEnemy());
    }
    
    private IEnumerator DestroyEnemy()
    {
        _entityAnimator.StartDieAnimation();
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
