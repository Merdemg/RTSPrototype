using UnityEngine;

public class ClosestEnemyStrategy : IMovementStrategy
{
    private readonly GridManager gridManager;
    private Unit currentTarget;

    public ClosestEnemyStrategy(GridManager gridManager)
    {
        this.gridManager = gridManager;
    }

    // Not looking for a new target while old one is alive should be ok for this prototype since they all spawn at start,
    // but for an actual RTS, we would compare any newly spawned, or any new targets emerging from fog of war to old target's distance
    public void Move(Unit unit)
    {
        if (currentTarget == null || currentTarget.IsDead)
        {
            currentTarget = FindClosestEnemy(unit);
            unit.SetTarget(currentTarget, "Closest target");
        }

        if (currentTarget != null)
        {
            Vector3 dir = (currentTarget.transform.position - unit.transform.position).normalized;
            unit.transform.position += dir * unit.MoveSpeed * Time.deltaTime;
        }
    }

    private Unit FindClosestEnemy(Unit unit)
    {
        var grid = gridManager.Grid;
        var (ux, uy) = grid.GetCellCoords(unit.transform.position);
        int width = grid.Width;
        int height = grid.Height;

        Unit closest = null;
        float closestSqrDist = float.MaxValue;

        int maxRadius = Mathf.Max(width, height);

        for (int r = 0; r < maxRadius; r++)
        {
            bool foundAny = false;

            for (int dx = -r; dx <= r; dx++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    // Only check perimeter cells of current radius
                    if (Mathf.Abs(dx) != r && Mathf.Abs(dy) != r)
                        continue;

                    int cx = ux + dx;
                    int cy = uy + dy;

                    if (!grid.InBounds(cx, cy))
                        continue;

                    GridCell cell = grid.GetCell(cx, cy);
                    if (cell == null)
                        continue;

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

            if (foundAny)
                break;
        }

        return closest;
    }
}
