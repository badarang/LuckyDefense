using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour, IAttackable
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
    
    [SerializeField]
    private List<Enemy> currentTargets = new List<Enemy>();

    private float attackCooldown = 0f;
    private Animator animator;
    private UnitAnimator unitAnimator;
    private UnitMovement unitMovement;
    private Transform flippable;

    #endregion

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        unitAnimator = GetComponentInChildren<UnitAnimator>();
        unitMovement = GetComponent<UnitMovement>();
        flippable = transform.Find("Flippable");
    }

    void Update()
    {
        if (unitMovement.IsDragging) return;
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f)
        {
            Attack();
            attackCooldown = 1f / attackSpeed;
        }
    }

    public void Attack()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);

        if (enemiesInRange.Length <= 0)
        {
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("SpecialAttack");
            return;
        }

        var isCritical = Random.value <= criticalChance;
        animator.SetTrigger((isCritical) ? "SpecialAttack" : "Attack");

        if (enemiesInRange[0].transform.position.x < transform.position.x)
            flippable.localScale = new Vector3(-1, 1, 1);
        else
            flippable.localScale = new Vector3(1, 1, 1);

        currentTargets.Clear();
        int attackCount = Mathf.Min(attackableUnitCount, enemiesInRange.Length);
        for (int i = 0; i < attackCount; i++)
        {
            Enemy enemy = enemiesInRange[i].GetComponent<Enemy>();
            if (enemy != null)
            {
                currentTargets.Add(enemy);
            }
        }
        
        if (unitAnimator.AttackAnimationLength > 0)
        {
            float frameRate = 12f;
            int totalFrames = Mathf.RoundToInt(unitAnimator.AttackAnimationLength * frameRate);
            int targetFrame = Mathf.Max(totalFrames - 2, 1);
            float targetTime = (float)targetFrame / frameRate;

            Invoke(nameof(OnHit), targetTime);
        }
        else
        {
            OnHit();
        }
    }

    /// <summary>
    /// Animation Event에서 호출되는 함수
    /// </summary>
    public void OnHit()
    {
        foreach (var enemy in currentTargets)
        {
            bool isAttackerRight = enemy.transform.position.x < transform.position.x;
            float finalDamage = (Random.value <= criticalChance) ? damage * 2f : damage;
            enemy.TakeDamage(finalDamage, isAttackerRight);
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
