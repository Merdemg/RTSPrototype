using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private List<DifficultyPreset> difficultyPresets;
    [SerializeField] TMP_Text difficultyDescription;
    [SerializeField] Canvas mainMenuCanvas;
    [SerializeField] TMP_Dropdown difficultyDropdown;
    [SerializeField] GameObject startButtonObj;
    [SerializeField] GameObject resumeButtonObj;

    private void OnEnable()
    {
        GameStateManager.OnGameStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState state)
    {
        mainMenuCanvas.enabled = (state == GameState.MainMenu || state == GameState.Paused);
        difficultyDropdown.interactable = state == GameState.MainMenu;
        startButtonObj.SetActive(state == GameState.MainMenu);
        resumeButtonObj.SetActive(state == GameState.Paused);
    }

    public void OnStrategyChanged(int index)
    {
        GameManager.Instance.Settings.aiType = (AIType)index;
        GameManager.Instance.ApplyCurrentDefenderStrategy();
    }

    public void OnDifficultyChanged(int index)
    {
        if (index < 0 || index >= difficultyPresets.Count)
            return;

        var preset = difficultyPresets[index];
        var settings = GameManager.Instance.Settings;

        settings.enemyHealthMultiplier = preset.enemyHealthMultiplier;
        settings.enemySpeedMultiplier = preset.enemySpeedMultiplier;

        difficultyDescription.text = $"Enemy Health: x{preset.enemyHealthMultiplier:0.##}\nEnemy Speed: x{preset.enemySpeedMultiplier:0.##}";

        Debug.Log($"Difficulty set to {preset.presetName}, health x{preset.enemyHealthMultiplier}, speed x{preset.enemySpeedMultiplier}");
    }

    public void OnExit()
    {
        Application.Quit();
    }

    public void OnStart()
    {
        GameManager.Instance.StartGame();
    }

    public void OnResume()
    {
        GameStateManager.Instance.TogglePause();
    }
}
