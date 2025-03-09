using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameEndPanel : MonoBehaviour
{
    [SerializeField] Button quitButton;
    
    public void Init()
    {
        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(() =>
        {
            GameManager.Instance.OnGameEnd?.Invoke();
        });
    }
}
