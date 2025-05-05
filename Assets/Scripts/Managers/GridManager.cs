using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Vector2Int gridSize = new Vector2Int(20, 20);
    [SerializeField] private float cellSize = 2f;
    [SerializeField] Vector3 gridOriginPoint = new Vector3(-100, 0, -100);

    private Grid<GridCell> grid;
    private readonly Dictionary<Unit, Vector3> trackedUnitPositions = new();

    public Grid<GridCell> Grid => grid;

    private void Awake()
    {
        grid = new Grid<GridCell>(gridSize.x, gridSize.y, cellSize, gridOriginPoint);
    }

    public void RegisterUnit(Unit unit)
    {
        if (unit == null) return;

        grid.RegisterUnit(unit);
        trackedUnitPositions[unit] = unit.transform.position;
    }

    public void UnregisterUnit(Unit unit)
    {
        if (unit == null) return;

        grid.UnregisterUnit(unit);
        trackedUnitPositions.Remove(unit);
    }

    public void MoveUnit(Unit unit, Vector3 lastPos)
    {
        grid.Move(unit, lastPos);
    }

    public void Clear()
    {
        foreach (var unit in trackedUnitPositions.Keys)
            grid.UnregisterUnit(unit);

        trackedUnitPositions.Clear();
    }

    public (int x, int y) GetCellCoords(Vector3 worldPosition) => grid.GetCellCoords(worldPosition);
    public Vector3 GetWorldPosition(int x, int y) => grid.GetWorldPosition(x, y);
    public bool InBounds(int x, int y) => grid.InBounds(x, y);
}
