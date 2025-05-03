using UnityEngine;

[System.Serializable]
public class EnemySpawnEntry
{
    public UnitType type;
    public int count = 1;
}

[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/Game Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Enemy Spawning")]
    public EnemySpawnEntry[] enemySpawnList;
    public float spawnRadius = 3f;
    //public float spawnDelay = 0f;

    [Header("Difficulty Settings")]
    [Range(0.1f, 3f)] public float enemySpeedMultiplier = 1f;
    [Range(0.1f, 3f)] public float enemyHealthMultiplier = 1f;

    [Header("Defenders")]
    [Range(1, 10)] public int defenderCount = 3;
    [Range(0.5f, 5f)] public float defenderSpacing = 2f;

    [Header("AI Behavior")]
    public AIType aiType = AIType.ClosestEnemy;
}