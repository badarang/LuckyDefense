using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public bool PlaceUpper;
    public PathCreator PathUpper;
    public PathCreator PathLower;
    private PathCreator currentPath;
    private Transform targetPoint;
    private int idx = 0;
    private Enemy _enemy;
    
    void Start()
    {
        currentPath = PlaceUpper ? PathUpper : PathLower;
        _enemy = GetComponent<Enemy>();
    }
    
    void Update()
    {
        if (_enemy.IsDead) return;
        if (targetPoint == null)
        {
            targetPoint = currentPath.GetNextPoint(ref idx);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, Time.deltaTime * _enemy.Speed);
            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                transform.position = targetPoint.position;
                targetPoint = currentPath.GetNextPoint(ref idx);
            }
        }
    }

    
}
