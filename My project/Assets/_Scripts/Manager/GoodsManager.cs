using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodsManager : Singleton<GoodsManager>
{
    private int gold;
    private int diamond;

    public void OnLoad()
    {
        Statics.DebugColor("GoodsManager Loaded", new Color(1, 1f, 0));
        gold = 0;
        diamond = 0;
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
}
