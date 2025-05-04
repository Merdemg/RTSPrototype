using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VanguardStrategy : IMovementStrategy
{
    private readonly GridManager gridManager;
    private readonly UnitRegistry unitRegistry;

    private Unit currentTarget;
    private GridCell targetCell;

    public VanguardStrategy(GridManager gridManager, UnitRegistry unitRegistry)
    {
        this.gridManager = gridManager;
        this.unitRegistry = unitRegistry;
    }

    /// <summary>
    /// O(N) cost only! (N is number of alive enemies)
    /// </summary>
    public void Move(Unit unit)
    {
        // Step 1: Clean up current cell if target is gone
        if (currentTarget == null || currentTarget.IsDead)
        {
            currentTarget = null;

            var (ux, uy) = gridManager.Grid.GetCellCoords(unit.transform.position);
            var currentCell = gridManager.Grid.GetCell(ux, uy);

            if (currentCell != null)
            {
                var localEnemies = currentCell.Units
                .Where(u => u.Faction != unit.Faction && !u.IsDead).ToList();

                if (localEnemies.Count > 0)
                {
                    currentTarget = localEnemies
                        .OrderBy(u => Vector3.Distance(unit.transform.position, u.transform.position))
                        .First();

                    targetCell = currentCell;
                    unit.SetTarget(currentTarget, "Cleaning own cell");
                }
            }
        }

        // Step 2: If still no target, do strategic targeting using registry
        if (currentTarget == null)
        {
            var enemies = unitRegistry.GetEnemies().Where(u => !u.IsDead).ToList();

            if (enemies.Count == 0)
            {
                unit.SetTarget(null, "No enemies");
                return;
            }

            // Find max speed
            float maxSpeed = enemies.Max(e => e.MoveSpeed);

            // Group fast enemies by cell
            var cellGroups = new Dictionary<GridCell, List<Unit>>();

            foreach (var enemy in enemies)
            {
                if (Mathf.Abs(enemy.MoveSpeed - maxSpeed) > 0.05f)
                    continue;

                var (x, y) = gridManager.Grid.GetCellCoords(enemy.transform.position);
                var cell = gridManager.Grid.GetCell(x, y);
                if (cell == null)
                    continue;

                if (!cellGroups.ContainsKey(cell))
                    cellGroups[cell] = new List<Unit>();

                cellGroups[cell].Add(enemy);
            }

            // Find cell with most fast enemies
            GridCell bestCell = null;
            int maxCount = 0;
            float bestDistToFlag = float.MaxValue;

            Vector3 flagWorldPos = GameManager.Instance.FlagPosition;

            foreach (var pair in cellGroups)
            {
                if (pair.Value.Count > maxCount)
                {
                    maxCount = pair.Value.Count;
                    bestCell = pair.Key;

                    var (x, y) = gridManager.Grid.GetCellCoords(bestCell);
                    Vector3 cellWorldPos = gridManager.Grid.GetWorldPosition(x, y);
                    bestDistToFlag = Vector3.SqrMagnitude(cellWorldPos - flagWorldPos);
                }
                else if (pair.Value.Count == maxCount)
                {
                    var (x, y) = gridManager.Grid.GetCellCoords(pair.Key);
                    Vector3 cellWorldPos = gridManager.Grid.GetWorldPosition(x, y);
                    float distToFlag = Vector3.SqrMagnitude(cellWorldPos - flagWorldPos);

                    if (distToFlag < bestDistToFlag)
                    {
                        bestCell = pair.Key;
                        bestDistToFlag = distToFlag;
                    }
                }
            }

            if (bestCell != null)
            {
                currentTarget = cellGroups[bestCell]
                    .OrderBy(u => Vector3.Distance(unit.transform.position, u.transform.position))
                    .FirstOrDefault();

                targetCell = bestCell;
                unit.SetTarget(currentTarget, $"Fastest: {maxSpeed:0.##}, in cell: {maxCount}");
            }
            else
            {
                unit.SetTarget(null, "No fast clusters");
            }
        }

        // Step 3: Move toward current target
        if (currentTarget != null)
        {
            Vector3 dir = (currentTarget.transform.position - unit.transform.position).normalized;
            unit.transform.position += dir * unit.MoveSpeed * Time.deltaTime;
        }
    }
}
