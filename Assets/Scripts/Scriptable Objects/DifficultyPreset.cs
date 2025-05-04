using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyPreset", menuName = "Scriptable Objects/Difficulty Preset")]
public class DifficultyPreset : ScriptableObject
{
    public string presetName = "Normal";

    [Range(0.1f, 3f)] public float enemyHealthMultiplier = 1f;
    [Range(0.1f, 3f)] public float enemySpeedMultiplier = 1f;
}
