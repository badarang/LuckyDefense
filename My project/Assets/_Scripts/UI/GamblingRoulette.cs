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

    private void SpinRoulette(bool isMyPlayer = true)
    {
        if (UnitManager.Instance.UnitCount >= Statics.InitialGameDataDic["MaxUnitCount"])
        {
            UIManager.Instance.UITextDictionary["GamblingUnitCountText"].GetComponent<TextAnimationBase>().ExpandAlert(Color.red);
            return;
        }
        
        if (GoodsManager.Instance.Diamond < Statics.GamblingCost[gamblingIdx])
        {
            UIManager.Instance.UITextDictionary["GamblingDiamondText"].GetComponent<TextAnimationBase>().ExpandAlert(Color.red);
            return;
        }
        StartCoroutine(SpinRouletteCO(isMyPlayer));
    }

    private IEnumerator SpinRouletteCO(bool isMyPlayer)
    {
        yield return new WaitForSeconds(.1f);
        var random = Random.Range(0, 100);
        if (random <= Statics.GamblingChance[gamblingIdx])
        {
            if (isMyPlayer) UIManager.Instance.CreateDisplayText("성공!", Color.green, new Vector3(0, -6, 0));
            UnitManager.Instance.SummonUnit(isMyPlayer: isMyPlayer, grade: Grade.Rare + gamblingIdx, usingGold: false);
        }
        else
        {
            if (isMyPlayer) UIManager.Instance.CreateDisplayText("실패...", Color.red, new Vector3(0, -6, 0));
        }
    }
}
