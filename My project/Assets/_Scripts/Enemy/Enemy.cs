using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Enemy : MonoBehaviour, IHittable
{
    private EntityAnimator _entityAnimator;
    [SerializeField] private float hp;
    [SerializeField] private float speed;
    public float Speed => speed;
    [SerializeField] private int dropGold;
    public bool IsDead { get; set; }

    private void OnEnable()
    {
        _entityAnimator = GetComponentInChildren<EntityAnimator>();
        _entityAnimator.InitEnemy(this);
        IsDead = false;
        RoundManager.Instance.AliveEnemies++;
    }
    
    private void OnDisable()
    {
        RoundManager.Instance.AliveEnemies--;
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
        StartCoroutine(DestroyEnemy());
    }
    
    private IEnumerator DestroyEnemy()
    {
        _entityAnimator.StartDieAnimation();
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
