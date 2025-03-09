using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Enemy : MonoBehaviour, IHittable
{
    private EntityAnimator _entityAnimator;
    [SerializeField] private float maxHp;
    [SerializeField] private float speed;
    [SerializeField] private EnemyType enemyType;
    private float hp;
    private float defenseRatio;
    public float DefenseRatio => defenseRatio;
    public float Speed => speed;
    
    [SerializeField] private GoodsType dropGoodsType;
    [SerializeField] private int dropGoodsValue;
    public bool IsDead { get; set; }
    public EnemyHpBar HpBar;
    private Vector3 originalScale;
    
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
        originalScale = (enemyType == EnemyType.Boss) ? Vector3.one * 2f : Vector3.one;
        HpBar.gameObject.SetActive(false);
        RoundManager.Instance.AliveEnemies++;
        if (RoundManager.Instance.AliveEnemies >= Statics.InitialGameDataDic["EnemyAlertThresHold"])
        {
            UIManager.Instance.ShowEnemyAlertPanel();
        }
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
        transform.DOScale(originalScale, .5f).SetEase(Ease.OutBack);
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
    
    public void Stun(float duration)
    {
        if (TryGetComponent(out EnemyMovement enemyMovement))
        {
            enemyMovement.StunCounter = duration;
        }
    }
    
    public void Die()
    {
        if (IsDead) return;
        IsDead = true;
        HpBar.gameObject.SetActive(false);

        if (dropGoodsType == GoodsType.Gold)
        {
            GoodsManager.Instance.Gold += dropGoodsValue;
            AIManager.Instance.Gold += dropGoodsValue;
        }
        else if (dropGoodsType == GoodsType.Diamond)
        {
            GoodsManager.Instance.Diamond += dropGoodsValue;
            AIManager.Instance.Diamond += dropGoodsValue;
        }

        RoundManager.Instance.AliveEnemies--;
        StartCoroutine(DestroyEnemy());
    }
    
    private IEnumerator DestroyEnemy()
    {
        _entityAnimator.StartDieAnimation();
        yield return new WaitForSeconds(1f);
        PoolManager.Instance.ReturnEnemy(gameObject);
    }
    
    public void SetHpMultiplier(int multiplier)
    {
        maxHp *= multiplier;
    }
    
    public void SetDefenseRatio(float ratio)
    {
        defenseRatio = ratio;
    }
}

public enum EnemyType
{
    Normal,
    Boss
}