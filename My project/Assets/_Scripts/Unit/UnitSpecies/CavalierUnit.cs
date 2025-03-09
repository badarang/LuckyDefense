using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class CavalierUnit : Unit
{
    public override void Attack()
    {
        base.Attack();
        if (Random.value < 0.1f)
        {
            currentTargets = GetCurrentTarget(Int32.MaxValue);
            if (currentTargets == null || currentTargets.Count == 0) return;

            //기절을 건다.
            foreach (var target in currentTargets)
            {
                target.Stun(3f);
            }
        }
    }
}
