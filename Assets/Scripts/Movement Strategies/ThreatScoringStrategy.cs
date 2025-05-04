using UnityEngine;

public class ThreatScoringStrategy : IMovementStrategy
{
    private readonly GridManager gridManager;
    private readonly Transform flagTransform;

    public ThreatScoringStrategy(GridManager gridManager, Transform flagTransform)
    {
        this.gridManager = gridManager;
        this.flagTransform = flagTransform;
    }

    public void Move(Unit unit)
    {
        Unit bestTarget = FindHighestThreatToFlag(unit);
        unit.SetTarget(bestTarget);

        if (bestTarget != null)
        {
            Vector3 dir = (bestTarget.transform.position - unit.transform.position).normalized;
            unit.transform.position += dir * unit.MoveSpeed * Time.deltaTime;
        }
    }

    private Unit FindHighestThreatToFlag(Unit requester)
    {
        Grid<GridCell> grid = gridManager.Grid;
        Vector3 flagPos = flagTransform.position;
        var (fx, fy) = grid.GetCellCoords(flagPos);
        int width = grid.Width;
        int height = grid.Height;

        Unit bestTarget = null;
        float bestScore = float.MinValue;
        int maxRadius = Mathf.Max(width, height);

        for (int r = 0; r < maxRadius; r++)
        {
            bool foundAny = false;

            for (int dx = -r; dx <= r; dx++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    if (Mathf.Abs(dx) != r && Mathf.Abs(dy) != r)
                        continue;

                    int cx = fx + dx;
                    int cy = fy + dy;

                    if (!grid.InBounds(cx, cy))
                        continue;

                    GridCell cell = grid.GetCell(cx, cy);
                    if (cell == null)
                        continue;

                    foreach (var other in cell.Units)
                    {
                        if (other == null || other == requester || other.Faction == requester.Faction)
                            continue;

                        float speed = other.MoveSpeed;
                        if (speed <= 0f) continue;

                        float distanceToFlag = Vector3.Distance(other.transform.position, flagPos);
                        float timeToFlag = distanceToFlag / speed;
                        float pointBonus = other.PointValue * 3f;

                        float score = pointBonus - timeToFlag;

                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestTarget = other;
                            foundAny = true;
                        }
                    }
                }
            }

            if (foundAny)
                break;
        }

        return bestTarget;
    }
}
