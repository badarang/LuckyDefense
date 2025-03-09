using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameState CurrentState { get; private set; }
    public Action OnGameStart;
    
    private void Awake()
    {
        base.Awake();
        OnLoad();
        CurrentState = GameState.Lobby;
    }
    
    private void OnLoad()
    {
        Statics.DebugColor("GameManager Loaded", new Color(0, .5f, 1f));
        UIManager.Instance.OnLoad();
        UnitManager.Instance.OnLoad();
        AIManager.Instance.OnLoad();
        GoodsManager.Instance.OnLoad();
        RoundManager.Instance.OnLoad();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CurrentState = GameState.InGame; 
            OnGameStart?.Invoke();
        }
    }
}


public enum GameState
{
    Lobby,
    InGame,
    Pause,
    InGameEnd,
}