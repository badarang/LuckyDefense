using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Statics
{
    public static List<int> UnitPickUpChance = new List<int> { 80, 15, 5 };
    public static List<int> GamblingChance = new List<int> { 50, 30, 15, 5 };
}

public enum Grade
{
    Normal,
    Rare,
    Epic,
    Mythic,
}