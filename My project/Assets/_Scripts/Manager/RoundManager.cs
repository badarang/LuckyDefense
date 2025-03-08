using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : Singleton<RoundManager>
{
    [Header("Rounds Settings")]
    [SerializeField] private List<Round> rounds;
    [SerializeField] private GameObject defaultEnemyPrefab;
    [SerializeField] private List<SpawnCircle> spawnCircles;

    [SerializeField] private GameObject enemySpawnGroup;
    public int currentRound = 1;
    private int aliveEnemies;
    private WaitForSeconds waveDelay = new WaitForSeconds(1f);

    public int AliveEnemies
    {
        set
        {
            aliveEnemies = value;
            CheckAliveEnemies();
        }
        get => aliveEnemies;
    }

    [SerializeField] private Transform upperStartPos;
    [SerializeField] private Transform lowerStartPos;
    private void Awake()
    {
        base.Awake();
        GenerateRounds();
    }

    private void Start()
    {
        GameManager.Instance.OnGameStart += OnGameStart;
    }
    
    public void OnLoad()
    {
        Statics.DebugColor("RoundManager Loaded", new Color(1, .5f, 0));
        currentRound = 1;
    }
    
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    
    private void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        rounds.Clear();
    }
    
    public void OnGameStart()
    {
        StartCoroutine(RepeatRound());
    }
    
    private IEnumerator RepeatRound()
    {
        if (currentRound > rounds.Count)
        {
            Debug.LogWarning($"라운드 수가 초과되었습니다. 현재 라운드: {currentRound}");
            yield break;
        }

        Round currentRoundData = rounds[currentRound - 1];
        float roundTime = currentRoundData.roundTime;
        Invoke(nameof(AlertFiveSec), roundTime - 5f);
        UIManager.Instance.SetRoundTime(roundTime);
        UIManager.Instance.SetRoundText(currentRound);
        StartCoroutine(SpawnWaves(currentRoundData));
        yield return new WaitForSeconds(roundTime);
        GoToNextRound();
        StartCoroutine(RepeatRound());
    }
    
    private IEnumerator SpawnWaves(Round currentRoundData)
    {
        foreach (var roundSpawnData in currentRoundData.roundSpawnData)
        {
            yield return new WaitForSeconds(roundSpawnData.spawnTime);
            foreach (var wavePreset in roundSpawnData.wavePresets)
            {
                foreach (var spawnData in wavePreset.spawnData)
                {
                    yield return waveDelay;
                    SpawnEnemy(spawnData.enemyPrefab, spawnData.isUpper);
                }
            }
        }
    }

    private void GetAndInitEnemy(GameObject enemyPrefab, bool isUpper = true)
    {
        var pos = isUpper ? upperStartPos.position : lowerStartPos.position;
        GameObject enemy = PoolManager.Instance.GetEnemy(enemyPrefab, pos);
        enemy.transform.parent = enemySpawnGroup.transform;
        enemy.SetActive(true);
        enemy.GetComponent<Enemy>().Init();
    }

    private void SpawnEnemy(GameObject enemyPrefab, int isUpper)
    {
        if (enemyPrefab == null)
        {
            enemyPrefab = defaultEnemyPrefab;
        }
        
        if (isUpper == -1) // 양쪽
        {
            GetAndInitEnemy(enemyPrefab, true);
            GetAndInitEnemy(enemyPrefab, false);
        }
        else if (isUpper == 0) // 하단
        {
            GetAndInitEnemy(enemyPrefab, false);
        }
        else if (isUpper == 1) // 상단
        {
            GetAndInitEnemy(enemyPrefab, true);
        }
    }
    
    private void CheckAliveEnemies()
    {
        UIManager.Instance.SetAliveEnemiesBar(AliveEnemies, Statics.InitialGameDataDic["MaxAliveEnemy"]);
    }

    private void GenerateRounds()
    {
        //TODO: Generate Round
    }
    
    public void GoToNextRound()
    {
        currentRound++;
    }
    
    private void AlertFiveSec()
    {
        foreach (var spawnCircle in spawnCircles)
        {
            spawnCircle.StartShowAlert(5);
        }
    }
}

[Serializable]
public class Round
{
    [Header("Round Properties")]
    public float roundTime;
    
    [Header("Wave Presets in this Round")]
    public List<RoundSpawnData> roundSpawnData;
    
}

[Serializable]
public class RoundSpawnData
{
    public float spawnTime;
    public List<WavePreset> wavePresets;
}