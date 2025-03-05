using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Statics
{
    public static List<int> UnitPickUpChance = new List<int> { 80, 15, 5 };
    public static List<int> GamblingChance = new List<int> { 50, 30, 15, 5 };
    
    public static Color GradeColor(Grade grade)
    {
        switch (grade)
        {
            case Grade.Normal:
                return Color.white;
            case Grade.Rare:
                return Color.green;
            case Grade.Epic:
                return Color.blue;
            case Grade.Mythic:
                return new Color(1, 0.5f, 0);
            default:
                return Color.white;
        }
    }
}

public enum Grade
{
    Normal,
    Rare,
    Epic,
    Mythic,
}

