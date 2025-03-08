using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpBar : MonoBehaviour
{
    Color fullHPColor = Color.green;
    Color midHPColor = Color.yellow;
    Color lowHPColor = Color.red;
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshPro hpText;

    public void Init(bool isBoss)
    {
        hpText.gameObject.SetActive(isBoss);
    }

    public void SetHpBar(float currentHp, float maxHp)
    {
        hpBar.fillAmount = currentHp / maxHp;
        hpText.text = $"{currentHp}";
        SetColor();
    }
    
    private void SetColor()
    {
        if (hpBar.fillAmount > 0.5f)
        {
            hpBar.color = Color.Lerp(midHPColor, fullHPColor, (hpBar.fillAmount - 0.5f) * 2);
        }
        else
        {
            hpBar.color = Color.Lerp(lowHPColor, midHPColor, hpBar.fillAmount * 2);
        }
    }

}
