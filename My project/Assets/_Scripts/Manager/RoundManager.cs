using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : Singleton<RoundManager>
{
    [Header("Rounds Settings")]
    [SerializeField] private List<Round> rounds;
    [SerializeField] private GameObject defaultEnemyPrefab;

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



    private void SpawnEnemy(GameObject enemyPrefab, int isUpper)
    {
        if (enemyPrefab == null)
        {
            enemyPrefab = defaultEnemyPrefab;
        }
        
        if (isUpper == -1) // 양쪽
        {
            GameObject enemy = Instantiate(enemyPrefab, upperStartPos.position, Quaternion.identity, enemySpawnGroup.transform);
            GameObject enemy2 = Instantiate(enemyPrefab, lowerStartPos.position, Quaternion.identity, enemySpawnGroup.transform);
            
            enemy.GetComponent<Enemy>().Init();
            enemy2.GetComponent<Enemy>().Init();
        }
        else if (isUpper == 0) // 하단
        {
            GameObject enemy = Instantiate(enemyPrefab, lowerStartPos.position, Quaternion.identity, enemySpawnGroup.transform);
            enemy.GetComponent<Enemy>().Init();
        }
        else if (isUpper == 1) // 상단
        {
            GameObject enemy = Instantiate(enemyPrefab, upperStartPos.position, Quaternion.identity, enemySpawnGroup.transform);
            enemy.GetComponent<Enemy>().Init();
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