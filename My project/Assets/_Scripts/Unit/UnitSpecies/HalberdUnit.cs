using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalberdUnit : Unit
{
    private void OnEnable()
    {
        UnitManager.Instance.UnitPropertyDic[UnitPropertyEnum.DefenseRatio] += 0.1f;
    }
    
    public override void OnDisable()
    {
        UnitManager.Instance.UnitPropertyDic[UnitPropertyEnum.DefenseRatio] -= 0.1f;
    }
}
