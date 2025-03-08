using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfo : MonoBehaviour
{
    [SerializeField] Transform unitIconTransform;
    [SerializeField] private TextMeshProUGUI unitName;
    [SerializeField] private TextMeshProUGUI unitSkillName;
    [SerializeField] private TextMeshProUGUI unitSkillDescription;
    [SerializeField] private TextMeshProUGUI unitSkillType;
    [SerializeField] private TextMeshProUGUI unitDamage;
    [SerializeField] private TextMeshProUGUI unitAttackSpeed;
    
    public void SetUnitInfo(Unit unit, int unitNum)
    {
        for (int i=0; i<3; i++)
        {
            unitIconTransform.GetChild(i).gameObject.SetActive(false);
        }
        
        for (int i=0; i<unitNum; i++)
        {
            unitIconTransform.GetChild(i).gameObject.SetActive(true);
            unitIconTransform.GetChild(i).GetComponent<Image>().sprite = unit.UnitIcon;
        }
        
        unitName.text = unit.UnitName;
        unitSkillName.text = unit.UnitSkillName;
        unitSkillDescription.text = unit.UnitSkillDescription;
        unitSkillType.text = unit.UnitSkillType.ToString();
        unitDamage.text = unit.Damage.ToString();
        unitAttackSpeed.text = unit.AttackSpeed.ToString();
    }
}
