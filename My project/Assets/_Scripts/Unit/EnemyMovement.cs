using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private bool placeUpper;
    public PathCreator PathUpper;
    public PathCreator PathLower;
    private PathCreator currentPath;
    private Transform targetPoint;
    private int idx = 0;
    private Enemy enemy;
    private Animator animator;
    private EntityAnimator entityAnimator;
    private GameObject flippable;
    private float stunCounter;
    public float StunCounter
    {
        get => stunCounter;
        set
        {
            stunCounter = value;
            if (stunCounter > 0)
            {
                IsWalking = false;
            }
            else
            {
                IsWalking = true;
            }
        }
    }

    private bool isWalking;

    public bool IsWalking
    {
        get => isWalking;
        set
        {
            isWalking = value;
            animator.SetTrigger(value ? "Walk" : "Idle");
        }
    }

    

    public void Init()
    {
        if (transform.position.y > -2f)
        {
            placeUpper = true;
        }
        else
        {
            placeUpper = false;
        }
        
        idx = 0;
        targetPoint = null;
        currentPath = placeUpper ? PathUpper : PathLower;
        enemy = GetComponent<Enemy>();
        animator = GetComponentInChildren<Animator>();
        entityAnimator = GetComponentInChildren<EntityAnimator>();
        flippable = transform.Find("Flippable").gameObject;
        StartCoroutine(StartWalkCO());
    }

    void Update()
    {
        if (currentPath == null) return;
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
        
        if (StunCounter > 0)
        {
            StunCounter -= Time.deltaTime;
            return;
        }
        
        if (!IsWalking || enemy.IsDead) return;
        if (targetPoint == null)
        {
            targetPoint = currentPath.GetNextPoint(ref idx);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, Time.deltaTime * enemy.Speed);
            var dir = -1;
            if (idx == 1 || idx == 2) dir = 1;
            flippable.transform.localScale = new Vector3(dir, 1, 1);
            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                transform.position = targetPoint.position;
                targetPoint = currentPath.GetNextPoint(ref idx);
            }
        }
    }
    
    private IEnumerator StartWalkCO()
    {
        yield return new WaitForSeconds(.5f);
        IsWalking = true;
        entityAnimator.StartWalkShakeAnimation();
    }

}
