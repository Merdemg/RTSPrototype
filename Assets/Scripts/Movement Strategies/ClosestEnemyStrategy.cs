using UnityEngine;

public class ClosestEnemyStrategy : IMovementStrategy
{
    private readonly GridManager gridManager;
    private Unit currentTarget;

    public ClosestEnemyStrategy(GridManager gridManager)
    {
        this.gridManager = gridManager;
    }

    public void Move(Unit unit)
    {
        currentTarget = FindClosestEnemy(unit);
        unit.SetTarget(currentTarget, "Closest target");

        if (currentTarget != null)
        {
            Vector3 dir = (currentTarget.transform.position - unit.transform.position).normalized;
            unit.transform.position += dir * unit.MoveSpeed * Time.deltaTime;
        }
    }

    private Unit FindClosestEnemy(Unit unit)
    {
        var grid = gridManager.Grid;
        var (unitX, unitY) = grid.GetCellCoords(unit.transform.position);
        int width = grid.Width;
        int height = grid.Height;

        Unit closest = null;
        float closestSqrDist = float.MaxValue;

        int maxRadius = Mathf.Max(width, height); // ensures full grid coverage if needed

        // Expand outward in concentric square shells from the unit's cell
        for (int currentRadius = 0; currentRadius < maxRadius; currentRadius++)
        {
            bool foundAny = false;

            for (int distanceX = -currentRadius; distanceX <= currentRadius; distanceX++)
            {
                for (int distanceY = -currentRadius; distanceY <= currentRadius; distanceY++)
                {
                    // Only check perimeter cells of current radius
                    if (Mathf.Abs(distanceX) != currentRadius && Mathf.Abs(distanceY) != currentRadius)
                        continue;

                    int cellX = unitX + distanceX;
                    int cellY = unitY + distanceY;

                    if (!grid.InBounds(cellX, cellY))
                        continue;

                    GridCell cell = grid.GetCell(cellX, cellY);
                    if (cell == null)
                        continue;

                    // Evaluate each enemy unit in the cell
                    foreach (var other in cell.Units)
                    {
                        if (other == null || other.IsDead || other == unit || other.Faction == unit.Faction)
                            continue;

                        float sqrDist = (other.transform.position - unit.transform.position).sqrMagnitude;
                        if (sqrDist < closestSqrDist)
                        {
                            closestSqrDist = sqrDist;
                            closest = other;
                            foundAny = true;
                        }
                    }
                }
            }

            // Stop expanding once a valid target is found
            if (foundAny)
                break;
        }

        return closest;
    }

}
