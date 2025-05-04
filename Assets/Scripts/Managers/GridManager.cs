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

    private float updateTimer = 0f;
    private const float UPDATE_INTERVAL = 0.5f;

    private void Awake()
    {
        grid = new Grid<GridCell>(gridSize.x, gridSize.y, cellSize, gridOriginPoint);
    }

    private void Update()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer >= UPDATE_INTERVAL)
        {
            UpdateTrackedUnits();
            updateTimer = 0f;
        }
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

    public void UpdateTrackedUnits()
    {
        // Copy keys to avoid modifying collection during iteration
        var keys = new List<Unit>(trackedUnitPositions.Keys);

        foreach (var unit in keys)
        {
            if (unit == null) continue;

            Vector3 lastPos = trackedUnitPositions[unit];
            Vector3 currentPos = unit.transform.position;

            if ((currentPos - lastPos).sqrMagnitude > 0.01f)
            {
                grid.Move(unit, lastPos);
                trackedUnitPositions[unit] = currentPos;
            }
        }
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
