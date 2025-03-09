using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseUnit : Unit
{
    private void OnEnable()
    {
        UnitManager.Instance.UnitPropertyDic[UnitPropertyEnum.Damage] += 0.2f;
    }
    
    public override void OnDisable()
    {
        base.OnDisable();
        UnitManager.Instance.UnitPropertyDic[UnitPropertyEnum.Damage] -= 0.2f;
    }
}
