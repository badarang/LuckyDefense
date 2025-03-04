using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Enemy : MonoBehaviour
{
    [SerializeField] private float hp;
    [SerializeField] private float speed;
    [SerializeField] private int dropGold;
}
