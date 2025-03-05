using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Enemy : MonoBehaviour, IHittable
{
    private UnitAnimator unitAnimator;
    [SerializeField] private float hp;
    [SerializeField] private float speed;
    [SerializeField] private int dropGold;
    
    private void Start()
    {
        unitAnimator = GetComponentInChildren<UnitAnimator>();
    }
    
    public void TakeDamage(float amount, bool isAttackerRight = true)
    {
        Debug.Log("TakeDamage");
        hp -= amount;
        unitAnimator.StartHitAnimation(isAttackerRight);
        if (hp <= 0)
        {
            Die();
        }
    }
    
    public void Die()
    {
        if (IsDead) return;
        GoodsManager.Instance.Gold += dropGold;
        StartCoroutine(DestroyEnemy());
    }
    
    private IEnumerator DestroyEnemy()
    {
        unitAnimator.StartDieAnimation();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color color = spriteRenderer.color;
        while (color.a > 0)
        {
            color.a -= 0.1f;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(.5f);
        Destroy(gameObject);
    }
    
    public bool IsDead => hp <= 0;
}
