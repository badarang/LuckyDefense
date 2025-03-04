using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    [SerializeField]
    private string unitName;
    
    [SerializeField]
    private Sprite unitSprite;
    
    [SerializeField]
    private Grade grade;
    
    [SerializeField]
    private float hp;

    [SerializeField]
    private float attackSpeed;

    [SerializeField]
    private float damage;

    [SerializeField]
    private float criticalChance;

    [SerializeField]
    private int sellPrice;

    [SerializeField]
    private Unit upgradeTo;
}