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
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemies.Add(enemy);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
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
            if (enemies.Count == 0) return;
            foreach (var enemy in enemies)
            {
                int damage = (int)this.damage;
                var isAttackerRight = transform.position.x > enemy.transform.position.x;
                enemy.TakeDamage(damage, isAttackerRight);
                UIManager.Instance.CreateDamageText(enemy.transform.position, damage, isCritical: true);
            }
            damageTimer = damageDelay;
        }
    }
    
}
