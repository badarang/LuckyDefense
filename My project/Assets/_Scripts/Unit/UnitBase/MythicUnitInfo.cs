using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mythic_", menuName = "Mythic/Mythic Unit Info")]
public class MythicUnitInfo : ScriptableObject
{
    public UnitTypeEnum mythicUnitType;
    public List<UnitTypeEnum> requiredUnits;
    public List<Sprite> requiredUnitIcons;
}