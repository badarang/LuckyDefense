using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoodsContainer : MonoBehaviour, IUITextBase
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI diamondText;
    [SerializeField] private TextMeshProUGUI aliveUnitText;

    public void ResetText()
    {
        goldText.text = $"{Statics.InitialGameDataDic["InitialGold"]}";
        diamondText.text = $"{Statics.InitialGameDataDic["InitialDiamond"]}";
        aliveUnitText.text = $"0/{Statics.InitialGameDataDic["MaxUnitCount"]}";
    }
}
