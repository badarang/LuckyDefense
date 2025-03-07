using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class UnitGroup
{
    public List<Unit> units = new List<Unit>();
    public Action<UnitGroup> OnUnitChanged;
    
    public UnitGroup()
    {
        OnUnitChanged += UpdateUnitPositions;
    }
    
    private void UpdateUnitPositions(UnitGroup unitGroup)
    {
        int unitCount = units.Count;

        Vector3 baseScale = Vector3.one;
        if (unitCount > 0)
        {
            baseScale = new Vector3(
                Mathf.Sign(units[0].Flippable.transform.localScale.x),
                1f,
                1f
            );
        }

        // 유닛 개수에 따라 위치 및 크기 조정
        if (unitCount == 1)
        {
            units[0].Flippable.transform.DOLocalMove(Vector3.zero, 0.2f);
            units[0].Flippable.transform.DOScale(baseScale, 0.2f);
        }
        else if (unitCount == 2)
        {
            units[0].Flippable.transform.DOLocalMove(new Vector3(-0.3f, 0, 0), 0.2f);
            units[1].Flippable.transform.DOLocalMove(new Vector3(0.3f, 0, 0), 0.2f);

            units[0].Flippable.transform.DOScale(baseScale * 0.85f, 0.2f);
            units[1].Flippable.transform.DOScale(baseScale * 0.85f, 0.2f);
        }
        else if (unitCount == 3)
        {
            units[0].Flippable.transform.DOLocalMove(new Vector3(0, 0.2f, 0), 0.2f);   // 중앙
            units[1].Flippable.transform.DOLocalMove(new Vector3(-0.3f, -0.2f, 0), 0.2f); // 왼쪽
            units[2].Flippable.transform.DOLocalMove(new Vector3(0.3f, -0.2f, 0), 0.2f);  // 오른쪽

            units[0].Flippable.transform.DOScale(baseScale * 0.7f, 0.2f);
            units[1].Flippable.transform.DOScale(baseScale * 0.7f, 0.2f);
            units[2].Flippable.transform.DOScale(baseScale * 0.7f, 0.2f);
        }
        else if (unitCount == 0)
        {
            units.Clear();
        }
    }
}