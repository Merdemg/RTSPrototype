using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] Canvas hudCanvas;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] GameObject debugPanel;
    [SerializeField] TMP_Text targetDebugText;

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

    private void Update()
    {
        if (!debugPanel.activeSelf) return;

        var defenders = UnitRegistry.Instance.GetDefenders();

        string debugPanelText = "";

        foreach (var defender in defenders)
        {
            string targetName = defender.Target ? defender.Target.Stats.unitName : "None";
            string reason = string.IsNullOrEmpty(defender.TargetReason) ? "–" : defender.TargetReason;
            debugPanelText += ($"{targetName}: {reason}\n");
        }
        targetDebugText.text = debugPanelText;
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

    public void OnDebugButtonClicked()
    {
        debugPanel.SetActive(!debugPanel.activeSelf);
    }
}
