using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    private TextMeshProUGUI damageText;

    public void Init()
    {
        damageText = GetComponent<TextMeshProUGUI>();
        StartCoroutine(DestroyText());
    }
    
    void Update()
    {
        //Move Up
        transform.position += new Vector3(0, 1f, 0) * Time.deltaTime;
    }
    
    IEnumerator DestroyText()
    {
        yield return new WaitForSeconds(.2f);
        StartCoroutine(FadeOut());
    }
    
    IEnumerator FadeOut()
    {
        var color = damageText.color;
        while (color.a > 0)
        {
            color.a -= Time.deltaTime * 5f;
            damageText.color = color;
            yield return null;
        }
        Destroy(gameObject);
    }
    
    public void SetText(int damage, bool isCritical)
    {
        //Set Text
        damageText.text = damage.ToString();
        
        //Set Color
        var lightRed = new Color(1, 0.3f, 0.2f);
        damageText.color = isCritical ? lightRed : Color.white;
        
        //Set Scale
        damageText.fontSize = isCritical ? 110 : 60;
    }
}
