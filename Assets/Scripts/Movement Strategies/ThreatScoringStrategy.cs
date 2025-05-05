using UnityEngine;

public class ThreatScoringStrategy : IMovementStrategy
{
    private readonly GridManager gridManager;
    private readonly Transform flagTransform;

    private Unit currentTarget;

    public ThreatScoringStrategy(GridManager gridManager, Transform flagTransform)
    {
        this.gridManager = gridManager;
        this.flagTransform = flagTransform;
    }

    // Not looking for a new target while old one is alive should be ok for this prototype since they all spawn at start,
    // but for an actual RTS, we would compare any newly spawned, or any new targets emerging from fog of war to old target's distance
    public void Move(Unit unit)
    {
        if (currentTarget == null || currentTarget.IsDead)
        {
            currentTarget = FindHighestThreatToFlag(unit);
            unit.SetTarget(currentTarget, "Highest threat");
        }

        if (currentTarget != null)
        {
            Vector3 dir = (currentTarget.transform.position - unit.transform.position).normalized;
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
                        if (other == null || other.IsDead || other == requester || other.Faction == requester.Faction)
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
