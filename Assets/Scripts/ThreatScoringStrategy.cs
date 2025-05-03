using UnityEngine;

public class ThreatScoringStrategy : IMovementStrategy
{
    private readonly GridManager gridManager;

    public ThreatScoringStrategy(GridManager gridManager)
    {
        this.gridManager = gridManager;
    }

    public void Move(Unit unit)
    {
        Grid<GridCell> grid = gridManager.Grid;
        var (ux, uy) = grid.GetCellCoords(unit.transform.position);

        Unit bestTarget = null;
        float bestScore = float.MinValue;

        foreach (GridCell cell in grid.All())
        {
            foreach (var other in cell.Units)
            {
                if (other == unit || other.Faction == unit.Faction)
                    continue;

                float distance = Vector3.Distance(unit.transform.position, other.transform.position);
                float speed = other.MoveSpeed;
                int points = other.PointValue;

                // Score = threat (speed + value) minus distance penalty
                float score = (points * 2f) + (speed * 3f) - (distance * 1.5f);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = other;
                }
            }
        }

        if (bestTarget != null)
        {
            Vector3 dir = (bestTarget.transform.position - unit.transform.position).normalized;
            unit.transform.position += dir * unit.MoveSpeed * Time.deltaTime;
        }
    }
}
