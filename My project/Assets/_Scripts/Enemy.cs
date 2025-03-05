using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Enemy : MonoBehaviour
{
    [SerializeField] private float hp;
    [SerializeField] private float speed;
    [SerializeField] private int dropGold;
    
    public void TakeDamage(float amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            Die();
        }
    }
    
    public void Die()
    {
        GoodsManager.Instance.Gold += dropGold;
        Destroy(gameObject);
    }
}
