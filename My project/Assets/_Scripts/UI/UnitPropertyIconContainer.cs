using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitPropertyIconContainer : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI valueText;
    
    public void SetIcon(Sprite sprite)
    {
        icon.sprite = sprite;
    }
    
    public void SetValue(float value)
    {
        valueText.text = $"{Mathf.RoundToInt(value * 100)}%";
    }
}
