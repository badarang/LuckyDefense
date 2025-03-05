using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AliveEnemyContainer : MonoBehaviour, IUITextBase
{
    [SerializeField] private TextMeshProUGUI aliveEnemyText;
    [SerializeField] private Image barImage;
    private float targetValue;
     
    public void ResetText()
    {
        aliveEnemyText.text = "0/100";
        ResetBar();
    }
    
    private void ResetBar()
    {
        barImage.fillAmount = 0;
        targetValue = 0;
    }
    
    public void SetBar(float _value)
    {
        targetValue = _value;
    }
    
    private void Update()
    {
        if (Mathf.Abs(barImage.fillAmount - targetValue) > 0.01f)
        {
            barImage.fillAmount = Mathf.Lerp(barImage.fillAmount, targetValue, Time.deltaTime);
        }
    }
    
    
}
