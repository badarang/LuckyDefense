using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitIconContainer : MonoBehaviour
{
    [SerializeField] private Image unitIcon;
    [SerializeField] private TextMeshProUGUI unitText;
    [SerializeField] private GameObject checkMark;
    public Button UnitButton;
    
    public void Init(Sprite icon, string text)
    {
        unitIcon.sprite = icon;
        unitText.text = text;
    }
    
    public void SetCheckMark(bool value)
    {
        checkMark.SetActive(value);
    }
    
    public void SetColor(Color color)
    {
        unitText.color = color;
    }

}
