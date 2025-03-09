using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public abstract class Unit : MonoBehaviour, IAttackable
{
    #region Unit Properties
    [Header("Unit Properties")]
    [SerializeField] private UnitTypeEnum unitType;

    public UnitTypeEnum UnitType
    {
        get => unitType;
        set => unitType = value;
    }
    
    [SerializeField] private GameObject circlePrefab;
    private Transform transformGUI;
    private Transform canvasGUI;
    private GameObject rangeGizmo;
    private Button sellButton;
    private Button upgradeButton;

    [SerializeField]
    private Grade grade;
    public Grade Grade => grade;
    
    [SerializeField]
    private float hp = 1;

    [SerializeField]
    private float attackSpeed = 1;
    public float AttackSpeed => attackSpeed;

    [SerializeField]
    private float range = 1;
    public float Range => range;

    [SerializeField]
    private float damage = 10;
    public float Damage => damage;

    [SerializeField]
    private float criticalChance = .1f;

    [SerializeField]
    private GoodsType goodsType;
    public GoodsType GoodsType => goodsType;
    
    [SerializeField]
    private int sellPrice;
    public int SellPrice => sellPrice;

    [SerializeField]
    private float moveSpeed = 5;
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
    private bool isMyPlayer;

    #endregion

    private void OnDisable()
    {
        StopAllCoroutines();
        isInitialized = false;
        if (isMyPlayer) UnitManager.Instance.UnitCount--;
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
        
        transformGUI = transform.Find("GUI");
        canvasGUI = transformGUI.Find("GUICanvas");
        rangeGizmo = transformGUI.Find("RangeGizmo").gameObject;
        sellButton = canvasGUI.Find("SellButton").GetComponent<Button>();
        upgradeButton = canvasGUI.Find("UpgradeButton").GetComponent<Button>();

        isInitialized = true;
        this.isMyPlayer = isMyPlayer;
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
    
    public GameObject GetRangeGizmo()
    {
        return rangeGizmo;
    }

    public void ToggleSelectedCircle(bool isShow)
    {
        circlePrefab.SetActive(isShow);
    }
    
    public void ToggleOutline(bool toggle)
    {
        if (!isMyPlayer) return;
        if (toggle)
        {
            _entityAnimator.ToggleOutline(true);
        }
        else
        {
            _entityAnimator.ToggleOutline(false);
        }
    }
    
    public void ToggleGUI(bool toggle, int unitNum)
    {
        if (toggle)
        {
            canvasGUI.GetComponent<UIAnimationBase>().Expand(Vector3.one * .005f);
            
            if (sellButton != null)
            {
                sellButton.onClick.RemoveAllListeners();
                sellButton.onClick.AddListener(() =>
                {
                    UnitManager.Instance.SellUnit(this);
                });
            }
            
            if (upgradeButton != null)
            {
                var condition = (unitNum >= Statics.InitialGameDataDic["MaxUnitGather"] && grade < Grade.Epic);
                upgradeButton.interactable = condition;
                upgradeButton.transform.Find("Lock").gameObject.SetActive(!condition);

                if (condition)
                {
                    upgradeButton.onClick.RemoveAllListeners();
                    upgradeButton.onClick.AddListener(() =>
                    {
                        UnitManager.Instance.UpgradeUnit(this);
                    }); 
                }
            }
            
            UIManager.Instance.PushGUIQueue(canvasGUI.gameObject);
        }
        else
        {
            canvasGUI.GetComponent<UIAnimationBase>().Shrink();
        }
    }

    public void ToggleDrawRange(bool toggle)
    {
        rangeGizmo.transform.localScale = new Vector3(range / 4, range / 4, 1);
        rangeGizmo.SetActive(toggle);
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
    스킬,
    액티브,
}
