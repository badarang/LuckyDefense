using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class KingUnit : Unit
{
    [SerializeField] private GameObject kingProjectilePrefab;
    [SerializeField] private GameObject kingDisplayProjectilePrefab;
    public override void Attack()
    {
        base.Attack();
        if (Random.value < 0.1f)
        {
            Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, range + 3f, enemyLayer);
    
            var nearestEnemy = GetNearestEnemy(enemiesInRange);
    
            if (nearestEnemy == null) return;

            GameObject projectile = Instantiate(kingProjectilePrefab, nearestEnemy.transform.position, Quaternion.identity);
            GameObject displayProjectile = Instantiate(kingDisplayProjectilePrefab, transform.position, Quaternion.identity);
    
            Destroy(projectile, 3f);
            Destroy(displayProjectile, 3f);
            
            var dir = (nearestEnemy.transform.position - transform.position).normalized.x;
            
            Tween rotationTween = displayProjectile.transform
                .DORotate(new Vector3(0, 0, 1080 * -dir), 0.8f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear);

            displayProjectile.transform.DOJump(nearestEnemy.transform.position, 1f, 1, 0.5f)
                .OnComplete(() =>
                {
                    rotationTween.Kill();
                    displayProjectile.transform.rotation = Quaternion.identity;
                    projectile.GetComponent<DamageCollider>().SetAttackSettings(damage * 10f, 0.5f);
                });
        }

    }
}
