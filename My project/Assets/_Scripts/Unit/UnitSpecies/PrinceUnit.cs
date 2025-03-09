using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class PrinceUnit : Unit
{
    [SerializeField] private GameObject projectilePrefab;

    public override void Attack()
    {
        base.Attack();
        if (Random.value < 0.1f)
        {
            Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, range + 3f, enemyLayer);
    
            var nearestEnemy = GetNearestEnemy(enemiesInRange);
    
            if (nearestEnemy == null) return;
            
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.GetComponent<DamageCollider>().SetAttackSettings(damage * 10f, 3f);

            var dir = (nearestEnemy.transform.position - transform.position).normalized;
            
            var targetPos = transform.position + dir * 3f;
            projectile.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            
            projectile.transform.DOMove(targetPos, 0.5f).OnComplete(() =>
            {
                projectile.GetComponent<SpriteRenderer>().DOFade(0, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    Destroy(projectile);
                });
            });
        }
    }
}
