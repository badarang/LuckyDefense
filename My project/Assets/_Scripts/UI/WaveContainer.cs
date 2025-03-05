using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveContainer : MonoBehaviour, IUITextBase
{
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] TextMeshProUGUI waveTimeText;
    
    public void ResetText()
    {
        waveText.text = "Wave 1";
        waveTimeText.text = "00:00";
    }
}
