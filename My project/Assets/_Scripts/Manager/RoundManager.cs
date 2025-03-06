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
    
    public void OnGameStart()
    {
        StartCoroutine(StartRound());
    }
    
    private IEnumerator StartRound()
    {
        if (currentRound > rounds.Count)
        {
            Debug.LogWarning($"라운드 수가 초과되었습니다. 현재 라운드: {currentRound}");
            yield break;
        }

        Round currentRoundData = rounds[currentRound - 1];
        float roundTime = currentRoundData.roundTime;
        UIManager.Instance.SetRoundTime(roundTime);

        float roundStartTime = Time.time;
        float elapsedTime = 0f;

        foreach (var roundSpawn in currentRoundData.roundSpawnData)
        {
            float spawnTime = roundSpawn.spawnTime;
            List<WavePreset> wavePresets = roundSpawn.wavePresets;
            float remainingTimeBeforeSpawn = Mathf.Max(0, spawnTime - (elapsedTime));

            if (remainingTimeBeforeSpawn > 0)
            {
                yield return new WaitForSeconds(remainingTimeBeforeSpawn);
            }

            foreach (var wavePreset in wavePresets)
            {
                List<WaveSpawnData> spawnData = wavePreset.spawnData;

                foreach (var waveSpawnData in spawnData)
                {
                    var delay = waveSpawnData.spawnTime;
                    GameObject enemyPrefab = waveSpawnData.enemyPrefab;
                    int isUpper = waveSpawnData.isUpper;

                    SpawnEnemy(enemyPrefab, isUpper);

                    elapsedTime += Mathf.Max(0.3f, delay);
                    yield return new WaitForSeconds(Mathf.Max(0.3f, delay));
                }
            }
        }

        yield return new WaitForSeconds(Mathf.Max(0, roundTime - elapsedTime));

        GoToNextRound();
        StartCoroutine(StartRound());
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
        }
        else if (isUpper == 0) // 하단
        {
            GameObject enemy = Instantiate(enemyPrefab, lowerStartPos.position, Quaternion.identity, enemySpawnGroup.transform);
        }
        else if (isUpper == 1) // 상단
        {
            GameObject enemy = Instantiate(enemyPrefab, upperStartPos.position, Quaternion.identity, enemySpawnGroup.transform);
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