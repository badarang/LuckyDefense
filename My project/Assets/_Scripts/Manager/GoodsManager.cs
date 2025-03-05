using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodsManager : Singleton<GoodsManager>
{
    private int gold;
    private int diamond;
    
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
