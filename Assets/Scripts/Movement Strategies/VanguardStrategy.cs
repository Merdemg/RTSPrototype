using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VanguardStrategy : IMovementStrategy
{
    private readonly GridManager gridManager;

    private Unit currentTarget;
    private GridCell targetCell;

    public VanguardStrategy(GridManager gridManager)
    {
        this.gridManager = gridManager;
    }

    /// <summary>
    /// O(R x P): max radius to find a target, P: number of perimeter cells in that radius
    /// </summary>
    public void Move(Unit unit)
    {
        var grid = gridManager.Grid;
        var (unitX, unitY) = grid.GetCellCoords(unit.transform.position);

        // Step 1: Clean up current cell if target is gone
        if (currentTarget == null || currentTarget.IsDead)
        {
            currentTarget = null;

            // Look in the current cell
            var currentCell = grid.GetCell(unitX, unitY);
            if (currentCell != null)
            {
                var localEnemies = currentCell.Units
                    .Where(u => u.Faction != unit.Faction && !u.IsDead)
                    .ToList();

                // If any enemy is in this cell, target the closest one
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

        // Step 2: Expand outward if no target found in own cell
        if (currentTarget == null)
        {
            int width = grid.Width;
            int height = grid.Height;
            int maxRadius = Mathf.Max(width, height);

            GridCell bestCell = null;
            int maxFastCount = 0;
            float maxEnemySpeedInRange = 0f;

            // Expand outward in square radius
            for (int currentRadius = 1; currentRadius < maxRadius; currentRadius++)
            {
                List<(GridCell cell, List<Unit> enemies)> candidates = new();

                // For each perimeter cell at this radius
                for (int distanceX = -currentRadius; distanceX <= currentRadius; distanceX++)
                {
                    for (int distanceY = -currentRadius; distanceY <= currentRadius; distanceY++)
                    {
                        if (Mathf.Abs(distanceX) != currentRadius && Mathf.Abs(distanceY) != currentRadius)
                            continue;

                        int cellX = unitX + distanceX;
                        int cellY = unitY + distanceY;

                        if (!grid.InBounds(cellX, cellY))
                            continue;

                        var cell = grid.GetCell(cellX, cellY);
                        if (cell == null) continue;

                        // Get enemies in this cell
                        var enemies = cell.Units
                            .Where(u => u.Faction != unit.Faction && !u.IsDead)
                            .ToList();

                        if (enemies.Count > 0)
                            candidates.Add((cell, enemies));
                    }
                }

                // If any enemies found in this radius
                if (candidates.Count > 0)
                {
                    // Determine max speed among all enemies in this ring
                    maxEnemySpeedInRange = candidates
                        .SelectMany(c => c.enemies)
                        .Max(u => u.MoveSpeed);

                    // Select the cell with most fast enemies
                    foreach (var (cell, enemies) in candidates)
                    {
                        int fastCount = enemies.Count(u => Mathf.Abs(u.MoveSpeed - maxEnemySpeedInRange) < 0.05f);

                        if (fastCount > maxFastCount)
                        {
                            bestCell = cell;
                            maxFastCount = fastCount;
                        }
                    }

                    break; // Stop expanding — we only use first ring with enemies
                }
            }

            // Step 3: Pick the closest fast enemy in the best cell
            if (bestCell != null)
            {
                currentTarget = bestCell.Units
                    .Where(u => u.Faction != unit.Faction && !u.IsDead && Mathf.Abs(u.MoveSpeed - maxEnemySpeedInRange) < 0.05f)
                    .OrderBy(u => Vector3.Distance(unit.transform.position, u.transform.position))
                    .FirstOrDefault();

                targetCell = bestCell;
                unit.SetTarget(currentTarget, $"Fast cluster at radius, fastCount: {maxFastCount}");
            }
            else
            {
                unit.SetTarget(null, "No enemies found");
            }
        }

        // Step 4: Move toward current target
        if (currentTarget != null)
        {
            Vector3 dir = (currentTarget.transform.position - unit.transform.position).normalized;
            unit.transform.position += dir * unit.MoveSpeed * Time.deltaTime;
        }
    }
}
