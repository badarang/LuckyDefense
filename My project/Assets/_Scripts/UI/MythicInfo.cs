using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MythicInfo : MonoBehaviour
{
    [SerializeField] private List<Unit> mythicUnits;
    
    [Header("Left")]
    [SerializeField] private List<UnitIconContainer> spawnableMythicUnits;

    [Header("Right")]
    [SerializeField] private GameObject mythicInfoPanel;
    [SerializeField] private TextMeshProUGUI mythicName;
    [SerializeField] private TextMeshProUGUI processText;
    [SerializeField] private List<UnitIconContainer> requiredUnits;
    [SerializeField] private Image mythicIcon;
    [SerializeField] private Button spawnButton;

    public void Init()
    {
        foreach (var unit in mythicUnits)
        {
            var unitType = unit.UnitType;
            var icon = unit.UnitIcon;
            var text = $"진행률 {UnitManager.Instance.GetProcess(unitType)}%";
            var container = spawnableMythicUnits[mythicUnits.IndexOf(unit)];
            container.Init(icon, text);
            
            container.UnitButton.onClick.RemoveAllListeners();
            container.UnitButton.onClick.AddListener(() =>
            {
                ShowUnitInfo(unit);
            });
        }
        
        //Default
        ShowUnitInfo(mythicUnits[0]);
    }
    
    private void ShowUnitInfo(Unit unit)
    {
        mythicInfoPanel.GetComponent<UIAnimationBase>().Expand();
        mythicName.text = unit.UnitName;
        var mythicUnitInfo = UnitManager.Instance.MythicUnitInfoDic[unit.UnitType];
        for (int i=0; i<requiredUnits.Count; i++)
        {
            var requiredUnit = mythicUnitInfo.requiredUnits[i];
            var icon = mythicUnitInfo.requiredUnitIcons[i];
            var container = requiredUnits[i];

            bool condition = UnitManager.Instance.GetUnitCount(requiredUnit) >= 1;
            var text = (condition) ? "보유" : "미보유";
            container.SetColor(condition ? Color.green : Color.red);
            container.SetCheckMark(condition);
            container.Init(icon, text);
        }
        
        mythicIcon.sprite = unit.UnitIcon;

        spawnButton.onClick.RemoveAllListeners();
        if (UnitManager.Instance.GetProcess(unit.UnitType) >= 100)
        {
            spawnButton.onClick.AddListener(() =>
            {
                UnitManager.Instance.UpgradeUnitMythic(unit.UnitType);
                GetComponent<UIAnimationBase>().Shrink();
                //TODO: 연출 
            });
        }
    }
}
