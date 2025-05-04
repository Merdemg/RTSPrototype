using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] Canvas hudCanvas;
    [SerializeField] TMP_Text scoreText;

    private void OnEnable()
    {
        GameStateManager.OnGameStateChanged += HandleStateChanged;
        ScoreManager.OnScoreChanged += UpdateScoreDisplay;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChanged -= HandleStateChanged;
        ScoreManager.OnScoreChanged -= UpdateScoreDisplay;
    }

    private void HandleStateChanged(GameState state)
    {
        hudCanvas.enabled = state == GameState.Playing;
    }

    private void UpdateScoreDisplay(int newScore)
    {
        scoreText.text = $"Score: {newScore}";
    }

    public void OnMenuButtonClicked()
    {
        GameStateManager.Instance.TogglePause();
    }
}
