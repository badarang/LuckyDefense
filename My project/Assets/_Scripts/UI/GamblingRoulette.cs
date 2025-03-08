using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamblingRoulette : MonoBehaviour
{
    [SerializeField]
    private Button pickUpButton;

    [SerializeField]
    private int gamblingIdx;

    private void OnEnable()
    {
        pickUpButton.onClick.RemoveAllListeners();
        pickUpButton.onClick.AddListener(() => { SpinRoulette(); });
    }

    private void SpinRoulette()
    {
        StartCoroutine(SpinRouletteCO());
    }

    private IEnumerator SpinRouletteCO()
    {
        //TODO: 룰렛 연출
        yield return new WaitForSeconds(.75f);
        var random = Random.Range(0, 100);
        if (random <= Statics.GamblingChance[gamblingIdx])
        {
            Statics.DebugColor("성공!", Color.green);
            UnitManager.Instance.SummonUnit(isMyPlayer: true, grade: Grade.Rare + gamblingIdx);
            //TODO: 성공 연출
        }
        else
        {
            Statics.DebugColor("실패!", Color.red);
            //TODO: 실패 연출
        }
    }
}
