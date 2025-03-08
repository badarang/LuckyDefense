using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnCircle : MonoBehaviour
{
    [SerializeField] TextMeshPro timeText;
    [SerializeField] UIAnimationBase uiAnimationBase;
    
    public void StartShowAlert(int time)
    {
        uiAnimationBase.Expand();
        StartCoroutine(ShowAlert(time));
    }
    
    IEnumerator ShowAlert(int time)
    {
        SetText(time.ToString());
        yield return new WaitForSeconds(1f);
        time--;
        if (time > 0)
        {
            StartCoroutine(ShowAlert(time));
        }
        else
        {
            uiAnimationBase.Shrink();
        }
    }
    
    public void SetText(string text)
    {
        this.timeText.text = text;
        timeText.GetComponent<TextAnimationBase>().ExpandAlert();
    }
}
