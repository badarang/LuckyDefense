using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : Singleton<RoundManager>
{
    [SerializeField] private List<Round> rounds;
    public int currentRound = 1;
    private int aliveEnemies;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    public void OnLoad()
    {
        Statics.DebugColor("RoundManager Loaded", new Color(1, .5f, 0));
        currentRound = 1;
    }
    
    public void GoToNextRound()
    {
        currentRound++;
    }
}

[Serializable] public class Wave
{
    public int waveNumber;
    public float waveTime;
    public List<Enemy> enemies;
}

[Serializable] public class Round
{
    public int roundNumber;
    public float roundTime;
    public List<Wave> waves;
    
}
