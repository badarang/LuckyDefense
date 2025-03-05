using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    #region Unit Properties
    [Header("Unit Properties")]
    [SerializeField]
    private string unitName;

    [SerializeField]
    private Grade grade;
    
    [SerializeField]
    private float hp;

    [SerializeField]
    private float attackSpeed;

    [SerializeField]
    private float range;

    [SerializeField]
    private float damage;

    [SerializeField]
    private float criticalChance;

    [SerializeField]
    private int sellPrice;

    [SerializeField]
    private Unit upgradeTo;
    
    [Header("Combat Settings")]
    [SerializeField]
    private int attackableUnitCount = 1;

    [SerializeField]
    private LayerMask enemyLayer;

    private float attackCooldown = 0f;
    private Animator animator;
    private Transform flippable;

    #endregion

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        flippable = transform.Find("Flippable");
    }

    void Update()
    {
        if (GetComponent<UnitMovement>().IsDragging) return;
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f)
        {
            Attack();
            attackCooldown = 1f / attackSpeed;
        }
    }

    void Attack()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);

        if (enemiesInRange.Length <= 0)
        {
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("SpecialAttack");
            return;
        }

        var isCritical = false;
        if (Random.value <= criticalChance) isCritical = true;
        
        animator.SetTrigger((isCritical) ? "SpecialAttack" : "Attack");
        if (enemiesInRange[0].transform.position.x < transform.position.x)
        {
            flippable.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            flippable.localScale = new Vector3(1, 1, 1);
        }

        int attackCount = Mathf.Min(attackableUnitCount, enemiesInRange.Length);
        for (int i = 0; i < attackCount; i++)
        {
            Enemy enemy = enemiesInRange[i].GetComponent<Enemy>();
            if (enemy != null)
            {
                float finalDamage = damage;
                if (isCritical) finalDamage *= 2f;
                enemy.TakeDamage(finalDamage);
            }
        }
        
        animator.SetTrigger("Idle");
    }

    public void Upgrade()
    {
        if (upgradeTo != null)
        {
            Instantiate(upgradeTo, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float amount)
    {
        //Not use
        hp -= amount;
        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
