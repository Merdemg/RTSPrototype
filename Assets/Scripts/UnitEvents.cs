using UnityEngine;

public static class UnitEvents
{
    public static event System.Action<Unit> OnEnemyUnitClicked;
    public static event System.Action<Unit> OnUnitSpawned;
    public static event System.Action<Unit> OnUnitDied;

    public static void RaiseEnemyUnitClicked(Unit enemy)
    {
        OnEnemyUnitClicked?.Invoke(enemy);
    }

    public static void RaiseUnitSpawned(Unit unit)
    {
        OnUnitSpawned?.Invoke(unit);
    }

    public static void RaiseUnitDied(Unit unit)
    {
        OnUnitDied?.Invoke(unit);
    }
}
