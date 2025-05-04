using UnityEngine;

public static class UnitEvents
{
    public static event System.Action<Unit> OnEnemyUnitClicked;

    public static void RaiseEnemyUnitClicked(Unit enemy)
    {
        OnEnemyUnitClicked?.Invoke(enemy);
    }
}
