using UnityEngine;
using System;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    Won,
    Lost
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public static event Action<GameState> OnGameStateChanged;

    private GameState currentState = GameState.MainMenu;
    public GameState CurrentState => currentState;

    public bool GameOver => currentState == GameState.Won || currentState == GameState.Lost;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetState(GameState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;
        OnGameStateChanged?.Invoke(currentState);

        Debug.Log($"Game state changed to: {currentState}");
    }

    public void TogglePause()
    {
        if (currentState == GameState.Playing)
            SetState(GameState.Paused);
        else if (currentState == GameState.Paused)
            SetState(GameState.Playing);
    }

    public void TriggerLoss()
    {
        if (GameOver) return;

        SetState(GameState.Lost);
    }

    public void TriggerWin()
    {
        if (GameOver) return;

        SetState(GameState.Won);
    }
}
