using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    public bool isExpand;
    public Vector3 expandScale;

    [Header("Attack Settings")]
    [SerializeField]
    private float damage;
    public float Damage => damage;

    [SerializeField]
    private float damageDelay;
    public float DamageDelay => damageDelay;
    private float damageTimer;
    
    private HashSet<Enemy> enemies = new HashSet<Enemy>();
    
    public void SetAttackSettings(float damage, float damageDelay)
    {
        GetComponent<Collider2D>().enabled = true;
        
        this.damage = damage;
        this.damageDelay = damageDelay;

        if (isExpand)
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(expandScale, .2f).SetEase(Ease.OutBack);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnTriggerEnter2D: " + other.name);
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemies.Add(enemy);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("OnTriggerExit2D: " + other.name);
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemies.Remove(enemy);
        }
    }
    
    private void OnDisable()
    {
        enemies.Clear();
    }
    
    private void Update()
    {
        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;
        }
        else
        {
            foreach (var enemy in enemies)
            {
                Debug.Log("DamageCollider: " + enemy);
                var isAttackerRight = transform.position.x > enemy.transform.position.x;
                enemy.TakeDamage(damage, isAttackerRight);
            }
            damageTimer = damageDelay;
        }
    }
    
}
