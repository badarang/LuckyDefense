using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    private TextMeshProUGUI damageText;

    void Start()
    {
        damageText = GetComponent<TextMeshProUGUI>();
        StartCoroutine(DestroyText());
    }
    
    void Update()
    {
        //Move Up
        transform.position += new Vector3(0, 1, 0) * Time.deltaTime;
    }
    
    IEnumerator DestroyText()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(FadeOut());
    }
    
    IEnumerator FadeOut()
    {
        var color = damageText.color;
        while (color.a > 0)
        {
            color.a -= Time.deltaTime;
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
        var lightRed = new Color(1, 0.3f, 0.5f);
        damageText.color = isCritical ? lightRed : Color.white;
        
        //Set Scale
        damageText.fontSize = isCritical ? 60 : 40;
    }
}
