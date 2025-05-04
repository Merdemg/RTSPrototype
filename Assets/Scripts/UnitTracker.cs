using UnityEngine;
using System.Collections.Generic;

public class UnitTracker : MonoBehaviour
{
    public static UnitTracker Instance { get; private set; }

    private readonly HashSet<Unit> enemyUnits = new();

    private void Awake()
    {
        Instance = this;
        UnitEvents.OnUnitSpawned += RegisterUnit;
        UnitEvents.OnUnitDied += UnregisterUnit;
    }

    private void OnDestroy()
    {
        UnitEvents.OnUnitSpawned -= RegisterUnit;
        UnitEvents.OnUnitDied -= UnregisterUnit;
    }

    private void RegisterUnit(Unit unit)
    {
        if (unit.Faction == Faction.Enemy)
            enemyUnits.Add(unit);
    }

    private void UnregisterUnit(Unit unit)
    {
        if (unit.Faction == Faction.Enemy)
        {
            enemyUnits.Remove(unit);

            if (enemyUnits.Count == 0 && !GameStateManager.Instance.GameOver)
            {
                GameStateManager.Instance.TriggerWin();
            }
        }
    }
}
