using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public static event Action<int> OnScoreChanged;

    private int currentScore = 0;
    public int CurrentScore => currentScore;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        UnitEvents.OnUnitDied += HandleUnitDeath;
    }

    private void OnDestroy()
    {
        UnitEvents.OnUnitDied -= HandleUnitDeath;
    }

    private void HandleUnitDeath(Unit unit)
    {
        if (unit.Faction == Faction.Enemy)
        {
            currentScore += unit.PointValue;
            OnScoreChanged?.Invoke(currentScore);
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
    }
}
