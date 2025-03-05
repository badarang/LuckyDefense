using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Statics
{
    public static List<int> UnitPickUpChance = new List<int> { 80, 15, 5 };
    public static List<int> GamblingChance = new List<int> { 50, 30, 15, 5 };
    public static float SpeedPerOneAnimation = .083f;
    
    public static Color GradeColor(Grade grade)
    {
        switch (grade)
        {
            case Grade.Normal:
                return new Color(1, 1, 1, 0.5f);
            case Grade.Rare:
                return new Color(0, 1, .3f, .5f);
            case Grade.Epic:
                return new Color(0, .4f, .8f, .5f);
            case Grade.Mythic:
                return new Color(1, 0.5f, 0, .5f);
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

