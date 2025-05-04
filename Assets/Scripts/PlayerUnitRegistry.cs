using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitRegistry : MonoBehaviour
{
    public static PlayerUnitRegistry Instance { get; private set; }

    private readonly List<Unit> defenders = new();

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
        if (unit.Faction == Faction.Player)
            defenders.Add(unit);
    }

    private void Unregister(Unit unit)
    {
        defenders.Remove(unit);
    }

    public List<Unit> GetDefenders()
    {
        return new List<Unit>(defenders); // return a copy to avoid external mutation
    }
}
