using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private List<DifficultyPreset> difficultyPresets;

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

        Debug.Log($"Difficulty set to {preset.presetName}, health x{preset.enemyHealthMultiplier}, speed x{preset.enemySpeedMultiplier}");
    }

    public void OnExit()
    {
        Application.Quit();
    }

    public void OnStart()
    {

    }
}
