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
        var (flagX, flagY) = grid.GetCellCoords(flagPos);
        int width = grid.Width;
        int height = grid.Height;

        Unit bestTarget = null;
        float bestScore = float.MinValue;
        int maxRadius = Mathf.Max(width, height); // ensures full map coverage if needed

        // Expand outward in concentric square shells from the flag
        for (int currentRadius = 0; currentRadius < maxRadius; currentRadius++)
        {
            bool foundAny = false;

            for (int distanceX = -currentRadius; distanceX <= currentRadius; distanceX++)
            {
                for (int distanceY = -currentRadius; distanceY <= currentRadius; distanceY++)
                {
                    // Only check perimeter of current radius
                    if (Mathf.Abs(distanceX) != currentRadius && Mathf.Abs(distanceY) != currentRadius)
                        continue;

                    int cellX = flagX + distanceX;
                    int cellY = flagY + distanceY;

                    if (!grid.InBounds(cellX, cellY))
                        continue;

                    GridCell cell = grid.GetCell(cellX, cellY);
                    if (cell == null)
                        continue;

                    // Evaluate each enemy unit in the cell
                    foreach (var other in cell.Units)
                    {
                        if (other == null || other.IsDead || other == requester || other.Faction == requester.Faction)
                            continue;

                        float speed = other.MoveSpeed;
                        if (speed <= 0f) continue;

                        // Estimate time to flag and use point value as bonus
                        float distanceToFlag = Vector3.Distance(other.transform.position, flagPos);
                        float timeToFlag = distanceToFlag / speed;
                        float pointBonus = other.PointValue * 3f;

                        float score = pointBonus - timeToFlag;

                        // Keep the best scoring target
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestTarget = other;
                            foundAny = true;
                        }
                    }
                }
            }

            // Stop expanding once at least one valid target is found
            if (foundAny)
                break;
        }

        return bestTarget;
    }

}
