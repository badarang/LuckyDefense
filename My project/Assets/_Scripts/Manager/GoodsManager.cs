using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodsManager : Singleton<GoodsManager>
{
    private int gold;
    private int diamond;
    private int requiredSummonGold;
    public int RequiredSummonGold => requiredSummonGold;

    public void OnLoad()
    {
        Statics.DebugColor("GoodsManager Loaded", new Color(1, 1f, 0));
        gold = Statics.InitialGameDataDic["InitialGold"];
        diamond = Statics.InitialGameDataDic["InitialDiamond"];
        requiredSummonGold = Statics.InitialGameDataDic["UnitRequiredGold"];
    }
    public int Gold
    {
        get => gold;
        set => gold = value;
    }
    
    public int Diamond
    {
        get => diamond;
        set => diamond = value;
    }
    
    public void IncreaseRequiredSummonGold()
    {
        requiredSummonGold += Statics.InitialGameDataDic["UnitRequiredGoldIncrease"];
    }
}


public enum GoodsType
{
    Gold,
    Diamond
}