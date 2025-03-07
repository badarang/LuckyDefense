using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    public GameObject unitPrefab;
    private const int Width = 6, Height = 3;
    private UnitGroup[,] upperUnitGroups = new UnitGroup[Width, Height];
    private UnitGroup[,] lowerUnitGroups = new UnitGroup[Width, Height];
    private float gridSize = 1.0f;
    
    public void OnLoad()
    {
        Statics.DebugColor("UnitManager Loaded", new Color(0, .8f, 0));
        InitializeUnitGroups();
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
        newUnit.Init(newUnitType, isMyPlayer);
        unitGroups[spawnPosition.x, spawnPosition.y].units.Add(newUnit);
        unitGroups[spawnPosition.x, spawnPosition.y].OnUnitChanged?.Invoke(unitGroups[spawnPosition.x, spawnPosition.y]);
    }
    
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
}

public class UnitGroup
{
    public List<Unit> units = new List<Unit>();
    public Action<UnitGroup> OnUnitChanged;
    
    public UnitGroup()
    {
        OnUnitChanged += UpdateUnitPositions;
    }
    
    private void UpdateUnitPositions(UnitGroup unitGroup)
    {
        int unitCount = units.Count;

        // 유닛 개수에 따라 위치 및 크기 조정
        if (unitCount == 1)
        {
            units[0].Flippable.transform.DOLocalMove(Vector3.zero, 0.2f);
            units[0].Flippable.transform.DOScale(Vector3.one, 0.2f);
        }
        else if (unitCount == 2)
        {
            units[0].Flippable.transform.DOLocalMove(new Vector3(-0.3f, 0, 0), 0.2f);
            units[1].Flippable.transform.DOLocalMove(new Vector3(0.3f, 0, 0), 0.2f);

            units[0].Flippable.transform.DOScale(Vector3.one * 0.8f, 0.2f);
            units[1].Flippable.transform.DOScale(Vector3.one * 0.8f, 0.2f);
        }
        else if (unitCount == 3)
        {
            units[0].Flippable.transform.DOLocalMove(new Vector3(0, 0.2f, 0), 0.2f);   // 중앙
            units[1].Flippable.transform.DOLocalMove(new Vector3(-0.3f, -0.2f, 0), 0.2f); // 왼쪽
            units[2].Flippable.transform.DOLocalMove(new Vector3(0.3f, -0.2f, 0), 0.2f);  // 오른쪽

            units[0].Flippable.transform.DOScale(Vector3.one * 0.6f, 0.2f);
            units[1].Flippable.transform.DOScale(Vector3.one * 0.6f, 0.2f);
            units[2].Flippable.transform.DOScale(Vector3.one * 0.6f, 0.2f);
        }
    }
}