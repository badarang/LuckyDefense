using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Statics
{
    public static List<int> UnitPickUpChance = new List<int> { 80, 15, 5 };
    public static List<int> GamblingChance = new List<int> { 60, 20 };
    public static float SpeedPerOneAnimation = .083f;
    public static Dictionary<string, int> InitialGameDataDic = new Dictionary<string, int>
    {
        {"UnitRequiredGold", 20},
        {"UnitRequiredGoldIncrease", 2},
        {"InitialGold", 1000},
        {"InitialDiamond", 0},
        {"MaxUnitCount", 20},
        {"MaxUnitLevel", 5},
        {"MaxUnitGrade", 4},
        {"MaxUnitGather", 3},
        {"MaxAliveEnemy", 100},
        {"EnemyAlertThresHold", 75},
    };

    public static Color GradeColor(Grade grade)
    {
        switch (grade)
        {
            case Grade.Common:
                return new Color(1, 1, 1, 0.5f);
            case Grade.Rare:
                return new Color(0, .4f, .8f, .5f);
            case Grade.Epic:
                return new Color(1, 0, 1, .5f);
            case Grade.Mythic:
                return new Color(1, 0.6f, 0, .5f);
            default:
                return Color.white;
        }
    }
    
    public static void DebugColor(string _text, Color _color)
    {
        Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(_color)}>{_text}</color>");
    }
}

public enum Grade
{
    Common,
    Rare,
    Epic,
    Mythic,
    None,
}