using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid<TCell> where TCell : class, new()
{
    private readonly int width;
    private readonly int height;
    private readonly float cellSize;
    private readonly Vector3 originPosition;

    private readonly TCell[,] cells;
    private readonly Dictionary<TCell, (int x, int y)> cellLookup = new();

    public int Width => width;
    public int Height => height;
    public float CellSize => cellSize;

    public Grid(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        cells = new TCell[width, height];

        // Pre-initialize cells
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                var cell = new TCell();
                cells[x, y] = cell;
                cellLookup[cell] = (x, y);
            }
    }

    public void AddCell(int x, int y, TCell cell)
    {
        if (!InBounds(x, y) || cell == null)
            return;

        cells[x, y] = cell;
        cellLookup[cell] = (x, y);
    }

    public TCell GetCell(int x, int y)
    {
        return InBounds(x, y) ? cells[x, y] : null;
    }

    public void DeleteCell(int x, int y)
    {
        if (!InBounds(x, y)) return;

        var cell = cells[x, y];
        if (cell != null)
            cellLookup.Remove(cell);

        cells[x, y] = null;
    }

    public void DeleteCell(TCell cell)
    {
        if (cell != null && cellLookup.TryGetValue(cell, out var pos))
        {
            cells[pos.x, pos.y] = null;
            cellLookup.Remove(cell);
        }
    }

    public void RegisterUnit(Unit unit)
    {
        var (x, y) = GetCellCoords(unit.transform.position);
        if (!InBounds(x, y)) return;

        (cells[x, y] as GridCell)?.AddUnit(unit);
    }

    public void UnregisterUnit(Unit unit)
    {
        var (x, y) = GetCellCoords(unit.transform.position);
        if (!InBounds(x, y)) return;

        (cells[x, y] as GridCell)?.RemoveUnit(unit);
    }

    public void Move(Unit unit, Vector3 oldPos)
    {
        var oldCoords = GetCellCoords(oldPos);
        var newCoords = GetCellCoords(unit.transform.position);

        if (oldCoords == newCoords) return;

        if (InBounds(oldCoords.x, oldCoords.y))
            (cells[oldCoords.x, oldCoords.y] as GridCell)?.RemoveUnit(unit);

        if (InBounds(newCoords.x, newCoords.y))
            (cells[newCoords.x, newCoords.y] as GridCell)?.AddUnit(unit);
    }

    public IEnumerable<TCell> All()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                var cell = cells[x, y];
                if (cell != null)
                    yield return cell;
            }
    }

    public (int x, int y) GetCellCoords(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - originPosition.x) / cellSize);
        int y = Mathf.FloorToInt((worldPosition.z - originPosition.z) / cellSize);
        return (x, y);
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x * cellSize, 0, y * cellSize) + originPosition;
    }

    public bool InBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }
}
