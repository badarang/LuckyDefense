using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyAlertPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI alertText;

    void Update()
    {
        alertText.text = $"{RoundManager.Instance.AliveEnemies} / {Statics.InitialGameDataDic["MaxAliveEnemy"]}";
    }
}
