using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameState CurrentState { get; private set; }
    public Action OnGameStart;
    public Action OnGameEnd;
    
    private void Awake()
    {
        base.Awake();
        OnLoad();
    }
    
    private void OnLoad()
    {
        CurrentState = GameState.Lobby;
        Statics.DebugColor("GameManager Loaded", new Color(0, .5f, 1f));
        UIManager.Instance.OnLoad();
        UnitManager.Instance.OnLoad();
        AIManager.Instance.OnLoad();
        GoodsManager.Instance.OnLoad();
        RoundManager.Instance.OnLoad();
        ParticleManager.Instance.OnLoad();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (CurrentState == GameState.Lobby)
            {
                CurrentState = GameState.InGame; 
                OnGameStart?.Invoke();
            }
        }
    }
    
    public void GameClear()
    {
        OnGameEnd?.Invoke();
        CurrentState = GameState.InGameEnd;
        UIManager.Instance.ShowGameClearPanel();
    }
    
    public void GameOver()
    {
        if (CurrentState == GameState.InGameEnd) return;
        OnGameEnd?.Invoke();
        CurrentState = GameState.InGameEnd;
        StartCoroutine(GameOverCO());
    }
    
    private IEnumerator GameOverCO()
    {
        Time.timeScale = 0.2f;
        yield return new WaitForSeconds(.6f);
        UIManager.Instance.ShowGameOverPanel();
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}


public enum GameState
{
    Lobby,
    InGame,
    Pause,
    InGameEnd,
}