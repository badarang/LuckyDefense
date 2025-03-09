using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DisplayText : MonoBehaviour
{
    [SerializeField] private TextMeshPro displayText;

    public void Init()
    {
        StartCoroutine(ReleaseText());
    }
    
    void Update()
    {
        //Move Up
        transform.position += new Vector3(0, 1f, 0) * Time.deltaTime;
    }
    
    IEnumerator ReleaseText()
    {
        yield return new WaitForSeconds(.2f);
        StartCoroutine(FadeOut());
    }
    
    IEnumerator FadeOut()
    {
        var color = displayText.color;
        while (color.a > 0)
        {
            color.a -= Time.deltaTime * 5f;
            displayText.color = color;
            yield return null;
        }
        PoolManager.Instance.ReturnDisplayText(gameObject);
    }
    
    public void SetText(int damage, bool isCritical)
    {
        //Set Text
        displayText.text = damage.ToString();
        
        //Set Color
        var lightRed = new Color(1, 0.3f, 0.2f);
        displayText.color = isCritical ? lightRed : Color.white;
        
        //Set Scale
        displayText.fontSize = isCritical ? 5 : 3;
    }
    
    public void SetText(string str, Color col)
    {
        displayText.text = str;
        displayText.color = col;
    }
}
