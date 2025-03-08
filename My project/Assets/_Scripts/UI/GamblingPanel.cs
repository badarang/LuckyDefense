using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamblingPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI diamondText;
    [SerializeField] private TextMeshProUGUI unitCountText;
    
    private TextMeshProUGUI originalDiamondText;
    private TextMeshProUGUI originalUnitCountText;
    
    public void Init()
    {
        originalDiamondText = UIManager.Instance.UITextDictionary["DiamondText"];
        originalUnitCountText = UIManager.Instance.UITextDictionary["UnitCountText"];
    }

    void Update()
    {
        if (originalDiamondText != null) diamondText.text = originalDiamondText.text;
        if (unitCountText != null) unitCountText.text = originalUnitCountText.text;
    }
}
