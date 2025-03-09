using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldUnit : Unit
{
    private void OnEnable()
    {
        UnitManager.Instance.UnitPropertyDic[UnitPropertyEnum.AttackSpeed] += 0.1f;
    }
    
    public override void OnDisable()
    {
        base.OnDisable();
        UnitManager.Instance.UnitPropertyDic[UnitPropertyEnum.AttackSpeed] -= 0.1f;
    }
}
