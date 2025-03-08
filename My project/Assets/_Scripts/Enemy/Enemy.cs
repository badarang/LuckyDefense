using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public abstract class Enemy : MonoBehaviour, IHittable
{
    private EntityAnimator _entityAnimator;
    [SerializeField] private float maxHp;
    [SerializeField] private float speed;
    [SerializeField] private EnemyType enemyType;
    private float hp;
    public float Speed => speed;
    [SerializeField] private int dropGold;
    public bool IsDead { get; set; }
    public EnemyHpBar HpBar;

    public void Init()
    {
        _entityAnimator = GetComponentInChildren<EntityAnimator>();
        _entityAnimator.Init();
        _entityAnimator.InitEnemy(this);
        if (TryGetComponent(out EnemyMovement enemyMovement))
        {
            enemyMovement.Init();
        }
        IsDead = false;
        hp = maxHp;
        HpBar.Init(enemyType == EnemyType.Boss);
        HpBar.gameObject.SetActive(false);
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
        HpBar.gameObject.SetActive(true);
        HpBar.SetHpBar(hp, maxHp);
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
        HpBar.gameObject.SetActive(false);
        GoodsManager.Instance.Gold += dropGold;
        UIManager.Instance.ChangeGoldText(GoodsManager.Instance.Gold);
        RoundManager.Instance.AliveEnemies--;
        StartCoroutine(DestroyEnemy());
    }
    
    private IEnumerator DestroyEnemy()
    {
        _entityAnimator.StartDieAnimation();
        yield return new WaitForSeconds(1f);
        PoolManager.Instance.ReturnEnemy(gameObject);
    }
}

public enum EnemyType
{
    Normal,
    Boss
}