using UnityEngine;
using System;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public bool GameOver { get; private set; }

    public static event Action OnGameOver;
    public static event Action OnGameWin;

    private void Awake()
    {
        Instance = this;
        GameOver = false;
    }

    public void TriggerLoss()
    {
        if (GameOver) return;

        GameOver = true;
        Debug.Log("Defeat! The flag was reached.");
        OnGameOver?.Invoke();
    }

    public void TriggerWin()
    {
        if (GameOver) return;

        GameOver = true;
        Debug.Log("Victory! All enemies eliminated.");
        OnGameWin?.Invoke();
    }
}
