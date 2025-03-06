using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave_", menuName = "Wave/Wave Preset")]
public class WavePreset : ScriptableObject
{
    public List<WaveSpawnData> spawnData;
}

[System.Serializable]
public class WaveSpawnData
{
    public float spawnTime;
    public GameObject enemyPrefab;
    public int isUpper = -1; // -1: 양쪽, 0: 하단, 1: 상단
}