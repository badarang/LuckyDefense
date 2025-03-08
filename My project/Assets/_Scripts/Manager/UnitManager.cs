using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitManager : Singleton<UnitManager>
{
    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    private GameObject destinationGUI;
    
    private const int Width = 6, Height = 3;
    private UnitGroup[,] upperUnitGroups = new UnitGroup[Width, Height];
    private UnitGroup[,] lowerUnitGroups = new UnitGroup[Width, Height];
    private int unitCount;
    private Dictionary<UnitTypeEnum, GameObject> spawnableUnitDic;
    public Dictionary <UnitTypeEnum, MythicUnitInfo> MythicUnitInfoDic;


    public int UnitCount {
        get => unitCount;
        set
        {
            unitCount = value;
            UIManager.Instance.ChangeUnitCountText(value);
        }
    }

    private bool isDragging = false;
    private Vector2Int dragFrom;
    private Vector2Int dragTo;

    private float gridSize = 1.0f;
    
    public void OnLoad()
    {
        Statics.DebugColor("UnitManager Loaded", new Color(0, .8f, 0));
        InitializeUnitGroups();
        InitializeLineRenderer();
        InitializeSpawnableUnitDic();
        InitializeMythicUnitInfoDic();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                //null check
                if (EventSystem.current.currentSelectedGameObject == null) return;
                if (EventSystem.current.currentSelectedGameObject.name == "SellButton" || EventSystem.current.currentSelectedGameObject.name == "UpgradeButton")
                {
                    return;
                }
            }
            UIManager.Instance.PopGUIQueue(setActive: false);
            UIManager.Instance.HideUnitInfo();
        }
        if (isDragging)
        {
            //dragFrom 에서 dragTo로 UI 선을 표시함. (노란색 점선)
            Vector2Int gridPos = GetMouseGridPosition();
            if (gridPos != dragTo)
            {
                dragTo = gridPos;
                destinationGUI.transform.position = GridToWorld(dragTo);
                destinationGUI.SetActive(true);
                UpdateDragLine();
                ToggleUnitSelectedCircle(dragFrom, true);
            }
        }
    }

    private void InitializeUnitGroups()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                upperUnitGroups[x, y] = new UnitGroup();
                lowerUnitGroups[x, y] = new UnitGroup();
            }
        }
    }
    
    private void InitializeLineRenderer()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        Color yellow = new Color(1, 0.98f, 0, .8f);
        lineRenderer.startColor = yellow;
        lineRenderer.endColor = yellow;
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = false;
    }
    
    private void InitializeSpawnableUnitDic()
    {
        spawnableUnitDic = new Dictionary<UnitTypeEnum, GameObject>();
        foreach (var unit in Resources.LoadAll<GameObject>("Prefabs/Units"))
        {
            spawnableUnitDic.Add(unit.GetComponent<Unit>().UnitType, unit);
        }
    }
    
    private void InitializeMythicUnitInfoDic()
    {
        MythicUnitInfoDic = new Dictionary<UnitTypeEnum, MythicUnitInfo>();
        foreach (var unit in Resources.LoadAll<MythicUnitInfo>("Data/MythicUnitInfo"))
        {
            MythicUnitInfoDic.Add(unit.mythicUnitType, unit);
        }
    }

    public void SummonUnit(bool isMyPlayer = true, Grade grade = Grade.None)
    {
        if (isMyPlayer)
        {
            var condition = unitCount < Statics.InitialGameDataDic["MaxUnitCount"];
            if (!condition)
            {
                UIManager.Instance.UITextDictionary["UnitCountText"].GetComponent<TextAnimationBase>().ExpandAlert(Color.red);
                return;
            }
        }
        
        GoodsManager.Instance.Gold -= GoodsManager.Instance.RequiredSummonGold;
        GoodsManager.Instance.IncreaseRequiredSummonGold();
        UIManager.Instance.ChangeRequiredGoldText(GoodsManager.Instance.RequiredSummonGold);
        
        var isUpper = !isMyPlayer;
        
        UnitTypeEnum newUnitType;
        //등급을 지정하지 않은 경우 랜덤으로 유닛을 소환
        if (grade == Grade.None) newUnitType = GetRandomUnitType();
        //등급을 지정한 경우 해당 등급의 유닛을 소환
        else newUnitType = GetUnitType(grade);
        
        Vector2Int spawnPosition = FindSpawnPosition(newUnitType, isMyPlayer);
        
        if (spawnPosition.x == -1)
        {
            Debug.LogWarning("소환할 수 있는 위치가 없습니다!");
            return;
        }
        
        UnitGroup[,] unitGroups = isUpper ? upperUnitGroups : lowerUnitGroups;

        Vector2 worldPosition = GridToWorld(spawnPosition, isMyPlayer);
        GameObject unitObj = Instantiate(spawnableUnitDic[newUnitType], worldPosition, Quaternion.identity);
        Unit newUnit = unitObj.GetComponent<Unit>();
        newUnit.Init(newUnitType, isMyPlayer, spawnPosition);
        unitGroups[spawnPosition.x, spawnPosition.y].units.Add(newUnit);
        unitGroups[spawnPosition.x, spawnPosition.y].OnUnitChanged?.Invoke(unitGroups[spawnPosition.x, spawnPosition.y]);
    }

    //업그레이드할 때 사용
    public Unit SummonUnit(Grade grade, Vector2Int spawnPosition, bool isMyPlayer = true)
    {
        UnitGroup[,] unitGroups = isMyPlayer ? lowerUnitGroups : upperUnitGroups;
        UnitTypeEnum newUnitType = GetUnitType(grade);
        
        GameObject unitObj = Instantiate(spawnableUnitDic[newUnitType], GridToWorld(spawnPosition), Quaternion.identity);
        Unit newUnit = unitObj.GetComponent<Unit>();
        newUnit.Init(newUnitType, true, spawnPosition);
        
        return newUnit;
    }
    
    //신화 유닛 소환 시 사용
    public Unit SummonUnit(UnitTypeEnum unitType, Vector2Int spawnPosition, bool isMyPlayer = true)
    {
        GameObject unitObj = Instantiate(spawnableUnitDic[unitType], GridToWorld(spawnPosition), Quaternion.identity);
        Unit newUnit = unitObj.GetComponent<Unit>();
        newUnit.Init(unitType, isMyPlayer, spawnPosition);
        
        return newUnit;
    }
    
    public void SellUnit(Unit unit, bool isMyPlayer = true)
    {
        Debug.Log("Sell Unit Called");
        UnitGroup[,] unitGroups = isMyPlayer ? lowerUnitGroups : upperUnitGroups;
        UnitGroup unitGroup = unitGroups[unit.GridPosition.x, unit.GridPosition.y];

        if (unitGroup.units.Count == 0) return;

        if (isMyPlayer)
        {
            if (unitGroup.units[0].GoodsType == GoodsType.Gold)
            {
                GoodsManager.Instance.Gold += (int)(GoodsManager.Instance.RequiredSummonGold * .2f);
            }
            else
            {
                GoodsManager.Instance.Diamond += unitGroup.units[0].SellPrice;
            }
        }

        unitGroup.units.Remove(unit);
        Destroy(unit.gameObject);
        unitGroup.OnUnitChanged?.Invoke(unitGroup);
        
        //X 위치에 있던 타입 A 유닛을 팔면, A 유닛 수가 2 이상일 때는 유닛이 계속 선택된다. (계속 팔 수 있음, 편의성 부분)
        if (isMyPlayer)
        {
            if (unitGroup.units.Count > 0)
            { 
                SelectPosition(unitGroup.units[0].GridPosition);
            }
            else
            {
                UIManager.Instance.HideUnitInfo();
            }
        }
        
        //판매 후 항상 유닛 수 3개로 유지하는 로직
        //만약 X 위치에 A 유닛이 3개 있고 Y 위치에 A 유닛이 1개 있다면, X 위치에서 1개를 팔았을 때 Y 위치에 있던 유닛이 X 위치로 이동해야함.
        KeyValuePair<UnitGroup, Unit> canMoveUnit = GetCanMoveUnitToAdjustMerge(unit, isMyPlayer);
        if (unitGroup.units.Count >= Statics.InitialGameDataDic["MaxUnitGather"] - 1 && canMoveUnit.Key != null)
        {
            var otherGroup = canMoveUnit.Key;
            var unitToMove = canMoveUnit.Value;
            var sellingGroup = unitGroup;
            var soldUnit = unit;
            
            unitToMove.GridPosition = soldUnit.GridPosition;
            Vector2 targetWorldPos = GridToWorld(soldUnit.GridPosition);
            unitToMove.GetComponent<UnitMovement>().StartMove(targetWorldPos, 20f);
                        
            sellingGroup.units.Add(unitToMove);
            otherGroup.units.Remove(unitToMove);
            
            otherGroup.OnUnitChanged?.Invoke(otherGroup);
            sellingGroup.OnUnitChanged?.Invoke(sellingGroup);
        }
    }
    
    private KeyValuePair<UnitGroup, Unit> GetCanMoveUnitToAdjustMerge(Unit soldUnit, bool isMyPlayer)
    {
        // lowerUnitGroups 배열 전체를 순회하며, 판매 위치가 아닌 다른 그리드에서 같은 타입 유닛을 찾음.
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (soldUnit.GridPosition == new Vector2Int(x, y))
                    continue;

                UnitGroup[,] unitGroups = isMyPlayer ?  lowerUnitGroups : upperUnitGroups;
                UnitGroup otherGroup = unitGroups[x, y];
                
                // 다른 그리드에 있는 유닛이 3마리 이상이면 움직일 필요 없음. (온전한 상태)
                if (otherGroup.units.Count >= Statics.InitialGameDataDic["MaxUnitGather"]) continue;
                
                if (otherGroup.units.Count > 0 && otherGroup.units[0].UnitType == soldUnit.UnitType)
                {
                    return new KeyValuePair<UnitGroup, Unit>(otherGroup, otherGroup.units[0]);
                }
            }
        }
        return new KeyValuePair<UnitGroup, Unit>(null, null);
    }
    
    public void UpgradeUnit(Unit unit, bool isMyPlayer = true)
    {
        Debug.Log("Upgrade Unit Called");
        UnitGroup[,] unitGroups = isMyPlayer ?  lowerUnitGroups : upperUnitGroups;
        UnitGroup unitGroup = unitGroups[unit.GridPosition.x, unit.GridPosition.y];
        if (unitGroup.units.Count < Statics.InitialGameDataDic["MaxUnitGather"]) return;
        if (unitGroup.units[0].Grade >= Grade.Epic) return;
        Grade newGrade = unitGroup.units[0].Grade + 1;
        
        //unitGroup에 있는 모든 유닛들을 제거
        
        unitGroup.units.ForEach(u => Destroy(u.gameObject));
        unitGroup.units.Clear();
        
        //새로운 유닛 생성
        Vector2Int originalPos = unit.GridPosition;
        Unit newUnit = SummonUnit(newGrade, originalPos, isMyPlayer);
        
        //이미 다른 그룹에 같은 타입 유닛이 있으면 그 그룹에 추가
        KeyValuePair<UnitGroup, Unit> targetUnitInfo = GetCanMoveUnitToAdjustMerge(newUnit, isMyPlayer);
        if (targetUnitInfo.Key != null)
        {
            //직접 이동해야 함.
            var targetUnit = targetUnitInfo.Value;
            var targetGroup = targetUnitInfo.Key;
            
            newUnit.GridPosition = targetUnit.GridPosition;
            Vector2 targetWorldPos = GridToWorld(targetUnit.GridPosition);
            newUnit.GetComponent<UnitMovement>().StartMove(targetWorldPos, 20f);
            
            targetGroup.units.Add(newUnit);
            targetGroup.OnUnitChanged?.Invoke(targetGroup);
        }
        else
        {
            unitGroups[unit.GridPosition.x, unit.GridPosition.y].units.Add(newUnit);
        }
        unitGroups[unit.GridPosition.x, unit.GridPosition.y].OnUnitChanged?.Invoke(unitGroups[unit.GridPosition.x, unit.GridPosition.y]);
    }

    public void UpgradeUnitMythic(UnitTypeEnum mythicUnit, bool isMyPlayer = true)
    {
        var mythicUnitInfo = MythicUnitInfoDic[mythicUnit];

        Vector2Int spawnPos = new Vector2Int(-1, -1);
        
        UnitGroup[,] unitGroups = isMyPlayer ? lowerUnitGroups : upperUnitGroups;
        foreach (var requiredUnit in mythicUnitInfo.requiredUnits)
        {
            foreach (var unit in unitGroups)
            {
                if (unit.units.Count > 0 && unit.units[0].UnitType == requiredUnit)
                {
                    var tmpPos = unit.units[0].GridPosition;
                    Destroy(unit.units[0].gameObject);
                    unit.units.RemoveAt(0);
                    unit.OnUnitChanged?.Invoke(unit);
                    
                    //기존 유닛이 있던 위치에 신화 유닛 소환하려고 시도 (높은 등급 우선)
                    if (spawnPos.x == -1 && unit.units.Count == 0) spawnPos = tmpPos;
                    break;
                }
            }
        }
        
        Unit newUnit = SummonUnit(mythicUnit, spawnPos, isMyPlayer);
        
        if (spawnPos.x == -1)
        {
            spawnPos = FindSpawnPosition(mythicUnit, isMyPlayer);
        }

        //신화 유닛은 단독으로 존재. (합성 불가)
        unitGroups[spawnPos.x, spawnPos.y].units.Add(newUnit);
        unitGroups[spawnPos.x, spawnPos.y].OnUnitChanged?.Invoke(unitGroups[spawnPos.x, spawnPos.y]);
    }

    private UnitTypeEnum GetRandomUnitType()
    {
        Grade grade = GetRandomGrade();
        return GetUnitType(grade);
    }

    private UnitTypeEnum GetUnitType(Grade grade)
    {
        List<UnitTypeEnum> sameGradeUnits = new List<UnitTypeEnum>();
        foreach (var unit in spawnableUnitDic.Keys)
        {
            var unitComponent = spawnableUnitDic[unit].GetComponent<Unit>();
            if (unitComponent.Grade == grade)
            {
                sameGradeUnits.Add(unitComponent.UnitType);
            }
        }
        
        if (sameGradeUnits.Count == 0)
        {
            Debug.LogWarning("해당 등급의 유닛이 없어 소드맨 유닛을 소환합니다.");
            return UnitTypeEnum.Sword;
        }
        
        int randomIndex = UnityEngine.Random.Range(0, sameGradeUnits.Count);
        return sameGradeUnits[randomIndex];
    }
    
    private Grade GetRandomGrade()
    {
        //TODO: 확률 조정
        int randomValue = UnityEngine.Random.Range(0, 100);
        if (randomValue < 50) return Grade.Common;
        if (randomValue < 70) return Grade.Rare;
        return Grade.Epic;
    }
    
    #region Unit Spawn System
    
    private Vector2Int FindSpawnPosition(UnitTypeEnum newUnitType, bool isMyPlayer)
    {
        bool isUpper = !isMyPlayer;
        List<Vector2Int> priorityPositions = GetPriorityPositions(isMyPlayer);
        UnitGroup[,] unitGroups = isUpper ? upperUnitGroups : lowerUnitGroups;
        
        Vector2Int spawnPosition = FindBestMergeablePosition(newUnitType, priorityPositions, unitGroups);

        if (spawnPosition.x == -1)
        {
            spawnPosition = FindEmptyPosition(priorityPositions, unitGroups);
        }

        return spawnPosition;
    }
    
    private List<Vector2Int> GetPriorityPositions(bool isMyPlayer)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        int startY = isMyPlayer ? 2 : 0;
        int endY = isMyPlayer ? 0 : 2;
        int stepY = isMyPlayer ? -1 : 1;
        
        for (int x = 0; x < Width; x++)
        {
            for (int y = startY; isMyPlayer ? y >= endY : y <= endY; y += stepY)
            {
                positions.Add(new Vector2Int(x, y));
            }
        }
        return positions;
    }
    
    private Vector2Int FindEmptyPosition(List<Vector2Int> priorityPositions, UnitGroup[,] unitGroups)
    {
        foreach (var pos in priorityPositions)
        {
            if (unitGroups[pos.x, pos.y].units.Count == 0)
                return pos;
        }

        return new Vector2Int(-1, -1);
    }
    
    private Vector2Int FindBestMergeablePosition(UnitTypeEnum newUnitType, List<Vector2Int> priorityPositions, UnitGroup[,] unitGroups)
    {
        //신화 유닛은 Merge 불가
        if (newUnitType == UnitTypeEnum.Cavalier || newUnitType == UnitTypeEnum.King) return new Vector2Int(-1, -1);
        foreach (var pos in priorityPositions)
        {
            if (unitGroups[pos.x, pos.y].units.Count > 0 && CanMerge(unitGroups[pos.x, pos.y].units, newUnitType))
                return pos;
        }

        return new Vector2Int(-1, -1);
    }

    private bool CanMerge(List<Unit> unitGroup, UnitTypeEnum newUnitType)
    {
        return unitGroup.Count < Statics.InitialGameDataDic["MaxUnitGather"] && unitGroup[0].UnitType == newUnitType;
    }

    private Vector2 GridToWorld(Vector2Int gridPos, bool isMyPlayer = true)
    {
        float worldX = gridPos.x - 2.5f;
        float worldY = gridPos.y + (isMyPlayer ? -5.8f : 1f);
        return new Vector2(worldX * gridSize, worldY * gridSize);
    }
    
    private Vector2Int WorldToGrid(Vector3 worldPos)
    {
        float x = Mathf.Round(worldPos.x + 2.5f) + .5f;
        float y = worldPos.y + 6.3f;
        return new Vector2Int((int)x, (int)y);
    }

    #endregion
    
    #region Unit Movement System
    
    private void UpdateDragLine()
    {
        Vector3 worldFrom = GridToWorld(dragFrom);
        Vector3 worldTo = GridToWorld(dragTo);
        
        lineRenderer.SetPosition(0, worldFrom);
        lineRenderer.SetPosition(1, worldTo);
    }
    
    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
    private Vector2Int GetMouseGridPosition()
    {
        Vector3 worldPos = GetMouseWorldPosition();
        worldPos.x = Mathf.Clamp(worldPos.x, -2.5f, 2.5f);
        worldPos.y = Mathf.Clamp(worldPos.y, -5.5f, -3.5f);
        return WorldToGrid(worldPos);
    }
    
    public void SelectPosition(Vector2Int gridPosition)
    {
        Debug.Log("Select Position Called");
        List<Unit> units = lowerUnitGroups[gridPosition.x, gridPosition.y].units;
        if (units.Count > 0)
        {
            UIManager.Instance.ShowUnitInfo(units[0], units.Count);
            Unit lastUnit = units[units.Count - 1];
            if (lastUnit == null) return;
            //신화 유닛은 합성, 판매 불가
            if (lastUnit.Grade >= Grade.Mythic) return;
            lastUnit.ToggleGUI(true, units.Count);
        }
    }

    public void StartDragPosition(Vector2Int gridPosition)
    {
        dragFrom = gridPosition;
        dragTo = gridPosition;
        lineRenderer.SetPosition(0, GridToWorld(dragFrom));
        lineRenderer.SetPosition(1, GridToWorld(dragTo));
        lineRenderer.enabled = true;
        isDragging = true;
        destinationGUI.transform.position = GridToWorld(dragTo);
        UIManager.Instance.PopGUIQueue(setActive: false);
    }
    
    public void EndDragPosition()
    {
        isDragging = false;
        lineRenderer.enabled = false;
        destinationGUI.SetActive(false);
        
        ToggleUnitSelectedCircle(dragFrom, false);

        if (dragFrom == dragTo)
        {
            //유닛을 선택한 경우
            SelectPosition(dragFrom);
        }
        else
        {
            //이동 요청
            MovePosition(dragFrom, dragTo, isMyPlayer: true);
        }
    }

    public void MovePosition(Vector2Int from, Vector2Int to, bool isMyPlayer = true)
    {
        UnitGroup[,] unitGroups = isMyPlayer ? lowerUnitGroups : upperUnitGroups;
        UnitGroup toGroup = unitGroups[to.x, to.y];

        if (toGroup.units.Count == 0)
        {
            //이동
            MoveEmptyPosition(from, to, isMyPlayer);
        }
        else
        {
            //Swap
            SwapPosition(from, to, isMyPlayer);
        }
    }
    
    private void MoveEmptyPosition(Vector2Int from, Vector2Int to, bool isMyPlayer = true)
    {
        UnitGroup[,] unitGroups = isMyPlayer ? lowerUnitGroups : upperUnitGroups;
        UnitGroup fromGroup = unitGroups[from.x, from.y];
        UnitGroup toGroup = unitGroups[to.x, to.y];
        
        Vector2 worldPosition = GridToWorld(to, isMyPlayer);
        foreach (var unit in fromGroup.units)
        {
            unit.GetComponent<UnitMovement>().StartMove(worldPosition);
            unit.GridPosition = to;
        }

        toGroup.units.AddRange(fromGroup.units);
        fromGroup.units.Clear();
        
        fromGroup.OnUnitChanged?.Invoke(fromGroup);
        toGroup.OnUnitChanged?.Invoke(toGroup);
    }
    
    private void SwapPosition(Vector2Int from, Vector2Int to, bool isMyPlayer = true)
    {
        UnitGroup[,] unitGroups = isMyPlayer ? lowerUnitGroups : upperUnitGroups;
        UnitGroup fromGroup = unitGroups[from.x, from.y];
        UnitGroup toGroup = unitGroups[to.x, to.y];
        
        Vector2 worldFromPosition = GridToWorld(from, isMyPlayer);
        Vector2 worldToPosition = GridToWorld(to, isMyPlayer);
        
        foreach (var unit in fromGroup.units)
        {
            unit.GetComponent<UnitMovement>().StartMove(worldToPosition);
            unit.GridPosition = to;
        }
        
        foreach (var unit in toGroup.units)
        {
            unit.GetComponent<UnitMovement>().StartMove(worldFromPosition);
            unit.GridPosition = from;
        }

        UnitGroup tmpGroup = new UnitGroup();
        tmpGroup.units.AddRange(fromGroup.units);
        fromGroup.units.Clear();
        fromGroup.units.AddRange(toGroup.units);
        toGroup.units.Clear();
        toGroup.units.AddRange(tmpGroup.units);
        
        fromGroup.OnUnitChanged?.Invoke(fromGroup);
        toGroup.OnUnitChanged?.Invoke(toGroup);
    }
    
    private void ToggleUnitSelectedCircle(Vector2Int gridPosition, bool isActive)
    {
        UnitGroup[,] unitGroups = lowerUnitGroups;
        UnitGroup unitGroup = unitGroups[gridPosition.x, gridPosition.y];
        foreach (var unit in unitGroup.units)
        {
            if (unit == null) continue;
            unit.ToggleSelectedCircle(isActive);
        }
    }
    
    public void DebugUnitGroups()
    {
        for (int y = 2; y >= 0; y--)
        {
            Debug.Log($"{y}층 : {lowerUnitGroups[0, y].units.Count} {lowerUnitGroups[1, y].units.Count} {lowerUnitGroups[2, y].units.Count} {lowerUnitGroups[3, y].units.Count} {lowerUnitGroups[4, y].units.Count} {lowerUnitGroups[5, y].units.Count}");
        }
    }
    
    
    #endregion
    
    #region Mythic Unit System
    
    public int GetProcess(UnitTypeEnum mythicUnitEnum, bool isMyPlayer = true)
    {
        int currentCount = 0;
        
        //Common : Rare : Epic = 1 : 3 : 6 비율을 따름
        UnitGroup[,] unitGroups = isMyPlayer ? lowerUnitGroups : upperUnitGroups;
        
        foreach (var requiredUnit in MythicUnitInfoDic[mythicUnitEnum].requiredUnits)
        {
            foreach (var unit in unitGroups)
            {
                if (unit.units.Count > 0 && unit.units[0].UnitType == requiredUnit)
                {
                    if (unit.units[0].Grade == Grade.Common) currentCount++;
                    else if (unit.units[0].Grade == Grade.Rare) currentCount += 3;
                    else if (unit.units[0].Grade == Grade.Epic) currentCount += 6;
                    break;
                }
            }
        }
        
        float process = (float)currentCount / 10;
        return (int)(process * 100);
    }
    
    public int GetUnitCount(UnitTypeEnum unitType, bool isMyPlayer = true)
    {
        UnitGroup[,] unitGroups = isMyPlayer ? lowerUnitGroups : upperUnitGroups;
        foreach (var unit in unitGroups)
        {
            if (unit.units.Count > 0 && unit.units[0].UnitType == unitType)
            {
                return unit.units.Count;
            }
        }
        return 0;
    }
    
    #endregion
}