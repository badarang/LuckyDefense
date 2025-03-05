using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SummonUnitContainer : MonoBehaviour, IUITextBase
{
    [SerializeField] private TextMeshProUGUI requiredGoldText;
    
    public void ResetText()
    {
        requiredGoldText.text = Statics.InitialGameDataDic["UnitRequiredGold"].ToString();
    }
}
