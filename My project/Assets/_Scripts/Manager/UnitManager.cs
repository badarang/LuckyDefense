using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    public GameObject unitPrefab;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject destinationGUI;
    private const int Width = 6, Height = 3;
    private UnitGroup[,] upperUnitGroups = new UnitGroup[Width, Height];
    private UnitGroup[,] lowerUnitGroups = new UnitGroup[Width, Height];
    
    
    private bool isDragging = false;
    private Vector2Int dragFrom;
    private Vector2Int dragTo;

    private float gridSize = 1.0f;
    
    public void OnLoad()
    {
        Statics.DebugColor("UnitManager Loaded", new Color(0, .8f, 0));
        InitializeUnitGroups();
        InitializeLineRenderer();
    }

    private void Update()
    {
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

    public void SummonUnit(bool isMyPlayer = true)
    {
        var isUpper = !isMyPlayer;
        UnitTypeEnum newUnitType = UnitTypeEnum.Sword;
        
        Vector2Int spawnPosition = FindSpawnPosition(newUnitType, isMyPlayer, isUpper);
        
        if (spawnPosition.x == -1)
        {
            Debug.LogWarning("소환할 수 있는 위치가 없습니다!");
            return;
        }
        
        UnitGroup[,] unitGroups = isUpper ? upperUnitGroups : lowerUnitGroups;

        Vector2 worldPosition = GridToWorld(spawnPosition, isMyPlayer);
        GameObject unitObj = Instantiate(unitPrefab, worldPosition, Quaternion.identity);
        Unit newUnit = unitObj.GetComponent<Unit>();
        newUnit.Init(newUnitType, isMyPlayer, spawnPosition);
        unitGroups[spawnPosition.x, spawnPosition.y].units.Add(newUnit);
        unitGroups[spawnPosition.x, spawnPosition.y].OnUnitChanged?.Invoke(unitGroups[spawnPosition.x, spawnPosition.y]);
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
            Debug.Log($"Empty Position: {spawnPosition}");

        }
        else
        {
            Debug.Log($"Merge Position: {spawnPosition}");
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
        foreach (var pos in priorityPositions)
        {
            if (unitGroups[pos.x, pos.y].units.Count > 0 && CanMerge(unitGroups[pos.x, pos.y].units, newUnitType))
                return pos;
        }

        return new Vector2Int(-1, -1);
    }

    private bool CanMerge(List<Unit> unitGroup, UnitTypeEnum newUnitType)
    {
        Debug.Log($"UnitGroup Count: {unitGroup.Count} / NewUnitType: {newUnitType}");
        return unitGroup.Count < 3 && unitGroup[0].UnitType == newUnitType;
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
        Statics.DebugColor($"Selected Position: {gridPosition}", new Color(.8f, 0.7f, 0));
        //TODO: UI에 선택된 위치 표시
    }

    public void StartDragPosition(Vector2Int gridPosition)
    {
        Statics.DebugColor($"Start Drag Position: {gridPosition}", new Color(.8f, 0.7f, 0));
        dragFrom = gridPosition;
        dragTo = gridPosition;
        lineRenderer.SetPosition(0, GridToWorld(dragFrom));
        lineRenderer.SetPosition(1, GridToWorld(dragTo));
        lineRenderer.enabled = true;
        isDragging = true;
        destinationGUI.transform.position = GridToWorld(dragTo);

        // List<Unit> units = lowerUnitGroups[gridPosition.x, gridPosition.y].units;
        // if (units.Count == 0) return;
        //
        // foreach (var unit in units)
        // {
        //     if (unit.TryGetComponent(out UnitMovement unitMovement))
        //     {
        //         unitMovement.StartDrag();
        //     }
        // }
    }
    
    public void EndDragPosition()
    {
        Statics.DebugColor($"End Drag Position: {dragTo}", new Color(.9f, 0.5f, 0));
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
        
        // List<Unit> units = lowerUnitGroups[dragFrom.x, dragFrom.y].units;
        // if (units.Count == 0) return;
        //
        // foreach (var unit in units)
        // {
        //     if (unit.TryGetComponent(out UnitMovement unitMovement))
        //     {
        //         unitMovement.EndDrag();
        //     }
        // }
    }

    public void MovePosition(Vector2Int from, Vector2Int to, bool isMyPlayer = true)
    {
        UnitGroup[,] unitGroups = isMyPlayer ? lowerUnitGroups : upperUnitGroups;
        UnitGroup toGroup = unitGroups[to.x, to.y];

        if (toGroup.units.Count == 0)
        {
            //이동
            Debug.Log("Move Empty Position");
            MoveEmptyPosition(from, to, isMyPlayer);
        }
        else
        {
            //Swap
            Debug.Log("Swap Position");
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
            Debug.Log($"Move Unit: {unit.UnitType} from {from} to {to}");
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