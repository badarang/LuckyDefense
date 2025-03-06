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

    

    void OnEnable()
    {
        if (transform.position.y > -2f)
        {
            placeUpper = true;
        }
        else
        {
            placeUpper = false;
        }
        
        currentPath = placeUpper ? PathUpper : PathLower;
        enemy = GetComponent<Enemy>();
        animator = GetComponentInChildren<Animator>();
        entityAnimator = GetComponentInChildren<EntityAnimator>();
        StartCoroutine(StartWalkCO());
    }

    void Update()
    {
        if (currentPath == null) return;
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
        if (!IsWalking || enemy.IsDead) return;
        if (targetPoint == null)
        {
            targetPoint = currentPath.GetNextPoint(ref idx);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, Time.deltaTime * enemy.Speed);
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
