using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Unit : MonoBehaviour, IAttackable
{
    #region Unit Properties
    [Header("Unit Properties")]
    private UnitTypeEnum unitType;
    public UnitTypeEnum UnitType => unitType;
    
    [SerializeField] private GameObject circlePrefab;

    [SerializeField]
    private Grade grade;
    public Grade Grade => grade;
    
    [SerializeField]
    private float hp;

    [SerializeField]
    private float attackSpeed;
    public float AttackSpeed => attackSpeed;

    [SerializeField]
    private float range;

    [SerializeField]
    private float damage;
    public float Damage => damage;

    [SerializeField]
    private float criticalChance;

    [SerializeField]
    private int sellPrice;

    [SerializeField]
    private Unit upgradeTo;

    [SerializeField]
    private float moveSpeed;
    public float MoveSpeed => moveSpeed;
    
    [Header("Unit Info")]
    public Sprite UnitIcon;
    public string UnitName;
    public string UnitSkillName;
    public string UnitSkillDescription;
    public UnitSkillTypeEnum UnitSkillType;

    public Vector2Int GridPosition { get; set; }
    
    [Header("Combat Settings")]
    [SerializeField]
    private int attackableUnitCount = 1;

    [SerializeField]
    private LayerMask enemyLayer;
    
    [SerializeField]
    private List<Enemy> currentTargets = new List<Enemy>();

    private float attackCooldown = 0f;
    private Animator animator;
    private EntityAnimator _entityAnimator;
    private UnitMovement unitMovement;
    private Transform flippable;
    public Transform Flippable => flippable;
    private Transform rendererTransform;
    private bool isInitialized = false;

    #endregion

    private void OnDisable()
    {
        StopAllCoroutines();
        isInitialized = false;
    }

    void Update()
    {
        if (!isInitialized || GameManager.Instance.CurrentState != GameState.InGame) return;
        if (unitMovement.IsDragging) return;
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f)
        {
            Attack();
            attackCooldown = 1f / attackSpeed;
        }
    }

    public void Init(UnitTypeEnum unitType, bool isMyPlayer, Vector2Int spawnPosition)
    {
        this.unitType = unitType;
        animator = GetComponentInChildren<Animator>();
        _entityAnimator = GetComponentInChildren<EntityAnimator>();
        _entityAnimator.Init();
        _entityAnimator.InitUnit(this);
        flippable = transform.Find("Flippable");
        rendererTransform = flippable.transform.Find("Renderer");
        unitMovement = GetComponent<UnitMovement>();
        unitMovement.Init(isMyPlayer);
        GridPosition = spawnPosition;

        isInitialized = true;
        
        if (isMyPlayer) UnitManager.Instance.UnitCount++;

    }

    public void Flip(bool isFacingRight)
    {
        var localScale = flippable.localScale;
        if (isFacingRight) localScale.x = Mathf.Abs(localScale.x);
        else localScale.x = -Mathf.Abs(localScale.x);
        flippable.localScale = localScale;
    }
    
    public void Attack()
    {
        if (unitMovement.IsDragging) return;
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
        
        currentTargets.Clear();
        int attackCount = Mathf.Min(attackableUnitCount, enemiesInRange.Length);
        for (int i = 0; i < attackCount; i++)
        {
            Enemy enemy = enemiesInRange[i].GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead)
            {
                currentTargets.Add(enemy);
            }
        }
        
        if (enemiesInRange.Length <= 0 || currentTargets.Count <= 0)
        {
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("SpecialAttack");
            return;
        }
        
        var isCritical = Random.value <= criticalChance;

        if (_entityAnimator.AttackAnimationLength > 0)
        {
            int targetFrame = Mathf.Max((int)_entityAnimator.AttackAnimationLength - 2, 1);
            float targetTime = (float)targetFrame * Statics.SpeedPerOneAnimation;

            StartCoroutine(AttackCoroutine(isCritical, targetTime));
        }
        else
        {
            OnHit(isCritical);
        }
        
        animator.SetTrigger((isCritical) ? "SpecialAttack" : "Attack");
        
        var isFacingRight = (currentTargets[0].transform.position.x < transform.position.x) ? false : true; 
        Flip(isFacingRight);
    }
    
    IEnumerator AttackCoroutine(bool isCritical, float targetTime)
    {
        yield return new WaitForSeconds(targetTime);
        OnHit(isCritical);
    }
    
    public void OnHit(bool isCritical)
    {
        foreach (var enemy in currentTargets)
        {
            if (enemy == null || enemy.IsDead) continue; //enemy가 도중에 죽은 경우
            bool isAttackerRight = enemy.transform.position.x < transform.position.x;
            int finalDamage = Mathf.RoundToInt((isCritical) ? damage * 2f : damage);
            UIManager.Instance.CreateDamageText(enemy.transform.position, finalDamage, isCritical);
            enemy.TakeDamage(finalDamage, isAttackerRight);
        }
        animator.SetTrigger("Idle");
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

    public void ToggleSelectedCircle(bool isShow)
    {
        circlePrefab.SetActive(isShow);
    }
    
    public void ToggleOutline(bool toggle)
    {
        if (toggle)
        {
            _entityAnimator.ToggleOutline(true);
        }
        else
        {
            _entityAnimator.ToggleOutline(false);
        }
    }
}


public enum UnitTypeEnum
{
    None,
    Sword,
    Shield,
    Horse,
    Spear,
    Halberd,
    Prince,
    Cavalier,
    King,
}

public enum UnitSkillTypeEnum
{
    패시브,
    액티브,
}
