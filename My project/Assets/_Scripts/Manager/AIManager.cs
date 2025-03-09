using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : Singleton<AIManager>
{
    [Header("Goods")]
    private int gold = 100;
    private int requiredGold = 20;
    private int diamond = 0;
    
    public int Gold
    {
        get => gold;
        set => gold = value;
    }
    public int Diamond
    {
        get => diamond;
        set => diamond = value;
    }
    
    [Header("Units")]
    private int unitCount = 0;
    public int UnitCount
    {
        get => unitCount;
        set => unitCount = value;
    }
    
    [Header("AI")]
    private float behaviourDelay = 1f;
    private float behaviourTimer = 2f;
    private const int Width = 6, Height = 3;
    //[SerializeField] private UnitGroup[,] targetUnitGroups = new UnitGroup[Width, Height];
    
    public void OnLoad()
    {
        Statics.DebugColor("AIManager Loaded", new Color(.2f, .2f, .6f));
    }

    private void Start()
    {
        //InitializeTargetUnitGroups();
    }
    
    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.InGame) return;

        if (behaviourTimer > 0f)
        {
            behaviourTimer -= Time.deltaTime;
        }
        else
        {
            behaviourTimer = behaviourDelay + Random.Range(0.5f, 1f);
            MakeAllUnitsMythical();
        }
    }

    private void MakeAllUnitsMythical()
    {
        //TODO: AI 고도화 작업 필요. (후순위)
        //전체 유닛이 신화가 아니라면
        if (!IsAllMythic())
        {
            //Debug.Log($"Gold {gold}, Diamond {diamond} Unit {unitCount}");
            
            //신화 소환이 가능하다면
            List<int> processList = new List<int>();
            foreach (var mythicUnitEnum in Statics.MythicUnitEnumList)
            {
                int process = UnitManager.Instance.GetProcess(mythicUnitEnum, isMyPlayer: false);
                processList.Add(process);
                if (process < 100) continue;
                UnitManager.Instance.UpgradeUnitMythic(mythicUnitEnum, isMyPlayer: false);
                return;
            }
            
            //업그레이드 가능한 유닛이 있다면
            var canUpgradePosition = UnitManager.Instance.GetCanUpgradePoition(isMyPlayer: false);
            if (canUpgradePosition.x != -1)
            {
                UnitManager.Instance.UpgradeUnit(canUpgradePosition, isMyPlayer: false);
                return;
            }

            //룰렛을 돌릴 수 있는지 확인
            var rouletteIdx = -1;
            var isDecided = false;
            //Epic을 만드는 것이 우선
            if (diamond >= 2) rouletteIdx = 1;
            
            if (diamond >= Statics.GamblingCost[0])
            {
                foreach (var process in processList)
                {
                    //Rare 유닛이 없어서 신화를 못 소환하는 경우
                    if (process == 90)
                    {
                        rouletteIdx = 0;
                        isDecided = true;
                        break;
                    }
                }
            }

            if (!isDecided && diamond >= Statics.GamblingCost[1])
            {
                foreach (var process in processList)
                {
                    //Epic 유닛이 없어서 신화를 못 소환하는 경우
                    if (process == 70)
                    {
                        rouletteIdx = 1;
                        break;
                    }
                }
            }
            
            if (rouletteIdx != -1)
            {
                AISpinRoulette(rouletteIdx);
                return;
            }

            //돈이 충분하다면
            if (gold >= requiredGold)
            {
                var changedPosition = new Vector2Int(-1, -1);
                //유닛이 최대치가 아니라면
                if (unitCount < Statics.InitialGameDataDic["MaxUnitCount"])
                {
                    changedPosition = UnitManager.Instance.SummonUnit(isMyPlayer: false);
                    unitCount++;
                    gold -= requiredGold;
                    requiredGold += Statics.InitialGameDataDic["UnitRequiredGoldIncrease"];
                }
                //유닛이 최대치라면
                else
                {
                    //등급이 낮은 유닛을 선택하여 제거
                    changedPosition = UnitManager.Instance.RemoveUnitOrderByGrade(isMyPlayer: false);
                    unitCount--;
                }

                if (changedPosition.x != -1)
                {
                    //변화가 있었다면, 위치 재배치
                    UnitManager.Instance.RepositionUnit(changedPosition, isMyPlayer: false);
                }
            }
        }
    }
    
    private bool IsAllMythic()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (UnitManager.Instance.UpperUnitGroups[x, y].units.Count == 0) return false;
                if (UnitManager.Instance.UpperUnitGroups[x, y].units[0].Grade != Grade.Mythic) return false;
            }
        }

        return true;
    }

    private void AISpinRoulette(int gamblingIdx)
    {
        diamond -= Statics.GamblingCost[gamblingIdx];
        
        var random = Random.Range(0, 100);
        if (random <= Statics.GamblingChance[gamblingIdx])
        {
            Statics.DebugColor($"AIManager Gambling Success with {gamblingIdx}" , Color.green);
            UnitManager.Instance.SummonUnit(isMyPlayer: false, grade: Grade.Rare + gamblingIdx);
        }
        else
        {
            Statics.DebugColor($"AIManager Gambling Fail with {gamblingIdx}" , Color.red);
        }
    }

    // private bool IsTargetUnitGroup()
    // {
    //     for (int x = 0; x < Width; x++)
    //     {
    //         for (int y = 0; y < Height; y++)
    //         {
    //             if (UnitManager.Instance.UpperUnitGroups[x, y].units.Count == 0) return false;
    //             if (UnitManager.Instance.UpperUnitGroups[x, y].units[0].UnitType !=
    //                 targetUnitGroups[x, y].units[0].UnitType) return false;
    //         }
    //     }
    //
    //     return true;
    // }
    // private void InitializeTargetUnitGroups()
    // {
    //     for (int x = 0; x < Width; x++)
    //     {
    //         for (int y = 0; y < Height; y++)
    //         {
    //             targetUnitGroups[x, y] = new UnitGroup();
    //             if (x % 2 == 0 && y != 1)
    //             {
    //                 CavalierUnit unit = new CavalierUnit();
    //                 unit.UnitType = UnitTypeEnum.Cavalier;
    //                 targetUnitGroups[x, y].units.Add(unit);
    //             }
    //             else if (x % 2 == 1 && y != 1)
    //             {
    //                 KingUnit unit = new KingUnit();
    //                 unit.UnitType = UnitTypeEnum.King;
    //                 targetUnitGroups[x, y].units.Add(unit);
    //             }
    //             else if (y == 1)
    //             {
    //                 if (x == 0 || x == 5)
    //                 {
    //                     KingUnit unit = new KingUnit();
    //                     unit.UnitType = UnitTypeEnum.King;
    //                     targetUnitGroups[x, y].units.Add(unit);
    //                 }
    //                 else
    //                 {
    //                     CavalierUnit unit = new CavalierUnit();
    //                     unit.UnitType = UnitTypeEnum.Cavalier;
    //                     targetUnitGroups[x, y].units.Add(unit);
    //                 }
    //             }
    //         }
    //     }
    // }
}

