using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpBar : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshPro hpText;

    public void Init(bool isBoss)
    {
        hpText.gameObject.SetActive(isBoss);
    }

    public void SetHpBar(float currentHp, float maxHp)
    {
        hpBar.fillAmount = currentHp / maxHp;
        hpText.text = $"{currentHp}/{maxHp}";
        SetColor();
    }
    
    private void SetColor()
    {
        if (hpBar.fillAmount > 0.7f)
        {
            hpBar.color = Color.green;
        }
        else if (hpBar.fillAmount > 0.3f)
        {
            hpBar.color = Color.yellow;
        }
        else
        {
            hpBar.color = Color.red;
        }
    }
}
