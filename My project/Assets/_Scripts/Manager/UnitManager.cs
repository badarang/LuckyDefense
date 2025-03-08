using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    private GameObject destinationGUI;

    [SerializeField] private LayerMask unitLayer;

    private const int Width = 6, Height = 3;
    private UnitGroup[,] upperUnitGroups = new UnitGroup[Width, Height];
    private UnitGroup[,] lowerUnitGroups = new UnitGroup[Width, Height];
    private int unitCount;
    private Dictionary<UnitTypeEnum, GameObject> spawnableUnitDic;
    

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
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsClickingOnUnit()) return;
            UIManager.Instance.HideUnitInfo();
            UIManager.Instance.PopGUIQueue(setActive: false);
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
            }
        }
    }
    
    private bool IsClickingOnUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, unitLayer);
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

    public void SummonUnit(bool isMyPlayer = true)
    {
        var isUpper = !isMyPlayer;
        UnitTypeEnum newUnitType = GetRandomUnitType();
        
        Vector2Int spawnPosition = FindSpawnPosition(newUnitType, isMyPlayer, isUpper);
        
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

    public void SummonUnit(Grade grade, Vector2Int spawnPosition)
    {
        UnitGroup[,] unitGroups = lowerUnitGroups;
        UnitTypeEnum newUnitType = GetUnitType(grade);
        
        GameObject unitObj = Instantiate(spawnableUnitDic[newUnitType], GridToWorld(spawnPosition), Quaternion.identity);
        Unit newUnit = unitObj.GetComponent<Unit>();
        newUnit.Init(newUnitType, true, spawnPosition);
        unitGroups[spawnPosition.x, spawnPosition.y].units.Add(newUnit);
        unitGroups[spawnPosition.x, spawnPosition.y].OnUnitChanged?.Invoke(unitGroups[spawnPosition.x, spawnPosition.y]);
    }
    
    public void SellUnit(Unit unit)
    {
        UnitGroup[,] unitGroups = lowerUnitGroups;
        UnitGroup unitGroup = unitGroups[unit.GridPosition.x, unit.GridPosition.y];

        if (unitGroup.units.Count == 0) return;
        unitGroup.units.Remove(unitGroup.units[unitGroup.units.Count - 1]);
        if (unitGroup.units[0].GoodsType == GoodsType.Gold)
        {
            GoodsManager.Instance.Gold += (int)(GoodsManager.Instance.RequiredSummonGold * .2f);
        }
        else
        {
            GoodsManager.Instance.Diamond += unitGroup.units[0].SellPrice;
        }
        
        //TODO: 여기서 추가 예외처리 해야함.
        //만약 X 위치에 A 유닛이 3개 있고 Y 위치에 A 유닛이 1개 있다면, X 위치에서 1개를 팔았을 때 Y 위치에 있던 유닛이 X 위치로 이동해야함.
        
        
        unitGroup.OnUnitChanged?.Invoke(unitGroup);
        Destroy(unit.gameObject);
    }
    
    public void UpgradeUnit(Unit unit)
    {
        UnitGroup[,] unitGroups = lowerUnitGroups;
        UnitGroup unitGroup = unitGroups[unit.GridPosition.x, unit.GridPosition.y];
        if (unitGroup.units.Count < Statics.InitialGameDataDic["MaxUnitGather"]) return;
        if (unitGroup.units[0].Grade >= Grade.Epic) return;
        Grade newGrade = unitGroup.units[0].Grade + 1;
        
        //unitGroup에 있는 모든 유닛들을 제거
        foreach (var u in unitGroup.units)
        {
            Destroy(u.gameObject);
        }
        
        //새로운 유닛 생성
        SummonUnit(newGrade, unit.GridPosition);
        
        unitGroup.OnUnitChanged?.Invoke(unitGroup);
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
    
    private Vector2Int FindSpawnPosition(UnitTypeEnum newUnitType, bool isMyPlayer, bool isUpper)
    {
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
        List<Unit> units = lowerUnitGroups[gridPosition.x, gridPosition.y].units;
        if (units.Count > 0)
        {
            UIManager.Instance.ShowUnitInfo(units[0], units.Count);
            Unit lastUnit = units[units.Count - 1];
            lastUnit.ToggleGUI(true);
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
    }
    
    public void EndDragPosition()
    {
        isDragging = false;
        lineRenderer.enabled = false;
        destinationGUI.SetActive(false);

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
    
    
    
    #endregion
}