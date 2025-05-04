using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class EndGameCanvasController : MonoBehaviour
{
    [SerializeField] Canvas endGameCanvas;
    [SerializeField] GameObject victoryPanel;
    [SerializeField] GameObject defeatPanel;
    [SerializeField] List<TMP_Text> enemiesEliminatedTextList = new List<TMP_Text>();

    private void OnEnable()
    {
        GameStateManager.OnGameStateChanged += HandleStateChanged;
        ScoreManager.OnEnemyStatsChanged += UpdateEnemiesEliminated;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChanged -= HandleStateChanged;
        ScoreManager.OnEnemyStatsChanged -= UpdateEnemiesEliminated;
    }

    public void OnRestartButtonClicked()
    {
        GameManager.Instance.RestartGame();
    }

    private void HandleStateChanged(GameState state)
    {
        endGameCanvas.enabled = state == GameState.Won || state == GameState.Lost;
        victoryPanel.SetActive(state == GameState.Won);
        defeatPanel.SetActive(state == GameState.Lost);

        if (state == GameState.Won || state == GameState.Lost)
        {
            UpdateEnemiesEliminated(ScoreManager.Instance.EnemiesKilled, ScoreManager.Instance.EnemiesSpawned);
        }
    }

    private void UpdateEnemiesEliminated(int killed, int total)
    {
        foreach (var text in enemiesEliminatedTextList)
        {
            text.text = $"Enemies Eliminated: {killed} / {total}";
        }
    }
}
