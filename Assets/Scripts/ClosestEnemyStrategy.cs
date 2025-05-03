using UnityEngine;

public class ClosestEnemyStrategy : IMovementStrategy
{
    private readonly GridManager gridManager;

    public ClosestEnemyStrategy(GridManager gridManager)
    {
        this.gridManager = gridManager;
    }

    public void Move(Unit unit)
    {
        Grid<GridCell> grid = gridManager.Grid;
        var (ux, uy) = grid.GetCellCoords(unit.transform.position);

        Unit closestTarget = null;
        float closestSqrDist = float.MaxValue;

        foreach (GridCell cell in grid.All())
        {
            foreach (var other in cell.Units)
            {
                if (other == unit || other.Faction == unit.Faction)
                    continue;

                float sqrDist = (other.transform.position - unit.transform.position).sqrMagnitude;
                if (sqrDist < closestSqrDist)
                {
                    closestSqrDist = sqrDist;
                    closestTarget = other;
                }
            }
        }

        if (closestTarget != null)
        {
            Vector3 dir = (closestTarget.transform.position - unit.transform.position).normalized;
            unit.transform.position += dir * unit.MoveSpeed * Time.deltaTime;
        }
    }
}
