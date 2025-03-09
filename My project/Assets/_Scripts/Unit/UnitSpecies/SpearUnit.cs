using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearUnit : Unit
{
    private void OnEnable()
    {
        UnitManager.Instance.UnitPropertyDic[UnitPropertyEnum.DefenseRatio] += 0.05f;
    }
    
    public override void OnDisable()
    {
        base.OnDisable();
        UnitManager.Instance.UnitPropertyDic[UnitPropertyEnum.DefenseRatio] -= 0.05f;
    }
}
