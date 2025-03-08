using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamblingRoulette : MonoBehaviour
{
    [SerializeField] private Button pickUpButton;
    
    private void OnEnable()
    {
        pickUpButton.onClick.RemoveAllListeners();
        pickUpButton.onClick.AddListener(() =>
        {
            Debug.Log("Pick Up Button Clicked");
        });
    }
}
