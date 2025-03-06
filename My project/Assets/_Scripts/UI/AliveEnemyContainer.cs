using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AliveEnemyContainer : MonoBehaviour, IUITextBase
{
    [SerializeField] private TextMeshProUGUI aliveEnemyText;
    [SerializeField] private Image barImage;
    [SerializeField] private BossIcon bossIcon;
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
    
    public void SetBar(int current, int max)
    {
        var _value = (float)current / max;
        targetValue = _value;
        aliveEnemyText.text = $"{current}/{max}";
        if (_value >= .75f)
        {
            bossIcon.ToggleBoss(true);
        }
        else
        {
            bossIcon.ToggleBoss(false);
        }
    }
    
    private void Update()
    {
        if (Mathf.Abs(barImage.fillAmount - targetValue) > 0.01f)
        {
            barImage.fillAmount = Mathf.Lerp(barImage.fillAmount, targetValue, Time.deltaTime);
        }
    }
    
    
}
