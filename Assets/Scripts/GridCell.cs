using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    private readonly List<Unit> units = new List<Unit>();

    public IReadOnlyList<Unit> Units => units;

    public void AddUnit(Unit unit)
    {
        if (unit != null && !units.Contains(unit))
            units.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        if (unit != null)
            units.Remove(unit);
    }

    public bool Contains(Unit unit)
    {
        return units.Contains(unit);
    }

    public void Clear()
    {
        units.Clear();
    }
}