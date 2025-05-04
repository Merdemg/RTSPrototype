using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public static event Action<int> OnScoreChanged;
    public static event Action<int, int> OnEnemyStatsChanged; // killed, total

    private int currentScore = 0;
    private int enemiesKilled = 0;
    private int enemiesSpawned = 0;
    public int CurrentScore => currentScore;
    public int EnemiesKilled => enemiesKilled;
    public int EnemiesSpawned => enemiesSpawned;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        UnitEvents.OnUnitDied += HandleUnitDeath;
        UnitEvents.OnUnitSpawned += HandleSpawned;
    }

    private void OnDestroy()
    {
        UnitEvents.OnUnitDied -= HandleUnitDeath;
        UnitEvents.OnUnitSpawned -= HandleSpawned;
    }

    private void HandleUnitDeath(Unit unit)
    {
        if (unit.Faction == Faction.Enemy)
        {
            enemiesKilled++;
            currentScore += unit.PointValue;
            OnScoreChanged?.Invoke(currentScore);
            OnEnemyStatsChanged?.Invoke(enemiesKilled, enemiesSpawned);
        }
    }

    private void HandleSpawned(Unit unit)
    {
        if (unit.Faction == Faction.Enemy)
        {
            enemiesSpawned++;
            OnEnemyStatsChanged?.Invoke(enemiesKilled, enemiesSpawned);
        }
    }

    public void ResetStats()
    {
        currentScore = 0;
        enemiesKilled = 0;
        enemiesSpawned = 0;

        OnScoreChanged?.Invoke(currentScore);
        OnEnemyStatsChanged?.Invoke(enemiesKilled, enemiesSpawned);
    }
}
