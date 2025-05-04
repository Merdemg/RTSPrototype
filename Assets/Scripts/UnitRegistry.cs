using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitRegistry : MonoBehaviour
{
    public static UnitRegistry Instance { get; private set; }

    private readonly HashSet<Unit> allUnits = new();

    private void Awake()
    {
        Instance = this;
        UnitEvents.OnUnitSpawned += Register;
        UnitEvents.OnUnitDied += Unregister;
    }

    private void OnDestroy()
    {
        UnitEvents.OnUnitSpawned -= Register;
        UnitEvents.OnUnitDied -= Unregister;
    }

    private void Register(Unit unit)
    {
        allUnits.Add(unit);
    }

    private void Unregister(Unit unit)
    {
        allUnits.Remove(unit);
    }

    public void DestroyAll()
    {
        var unitsToDestroy = new List<Unit>(allUnits);

        foreach (var unit in unitsToDestroy)
        {
            if (unit != null)
                unit.ForceCleanup(); // TODO: Add an object pool?
        }

        allUnits.Clear();
    }

    public List<Unit> GetAll() => new(allUnits);
    public List<Unit> GetDefenders() => new(allUnits.Where(u => u.Faction == Faction.Player));
    public List<Unit> GetEnemies() => new(allUnits.Where(u => u.Faction == Faction.Enemy));

}
