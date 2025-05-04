using System.Collections.Generic;
using UnityEngine;

public class RearguardStrategy : IMovementStrategy
{
    private readonly GridManager gridManager;
    private readonly Transform flagTransform;
    private const int SEARCH_RADIUS = 2;
    private const float FLAG_GUARD_RADIUS = 1.5f;

    public RearguardStrategy(GridManager gridManager, Transform flagTransform)
    {
        this.gridManager = gridManager;
        this.flagTransform = flagTransform;
    }

    /// <summary>
    /// O(R + T) R is number of cells in range, T is total number of units in range
    /// </summary>
    public void Move(Unit unit)
    {
        Unit currentTarget = unit.Target;

        // Reacquire if no target or target is dead
        if (currentTarget == null || currentTarget.IsDead)
        {
            currentTarget = null;
            float bestETA = float.MaxValue;

            var flagCoords = gridManager.Grid.GetCellCoords(flagTransform.position);
            var candidateCells = gridManager.Grid.GetCellsInRadius(flagCoords, SEARCH_RADIUS);

            foreach (var cell in candidateCells)
            {
                foreach (var other in cell.Units)
                {
                    if (other == unit || other.Faction == unit.Faction || other.IsDead)
                        continue;

                    float distToFlag = Vector3.Distance(other.transform.position, flagTransform.position);
                    float eta = distToFlag / Mathf.Max(other.MoveSpeed, 0.01f);

                    if (eta < bestETA)
                    {
                        bestETA = eta;
                        currentTarget = other;
                    }
                }
            }

            if (currentTarget != null)
            {
                unit.SetTarget(currentTarget, $"ETA: {bestETA:0.0}");
            }
        }

        // Move to target if valid
        if (currentTarget != null && !currentTarget.IsDead)
        {
            Vector3 dir = (currentTarget.transform.position - unit.transform.position).normalized;
            unit.transform.position += dir * unit.MoveSpeed * Time.deltaTime;
        }
        else
        {
            // Move back to flag if strayed too far
            float distToFlag = Vector3.Distance(unit.transform.position, flagTransform.position);

            if (distToFlag > FLAG_GUARD_RADIUS)
            {
                Vector3 dir = (flagTransform.position - unit.transform.position).normalized;
                unit.transform.position += dir * unit.MoveSpeed * Time.deltaTime;
                unit.SetTarget(null, "Returning to flag");
            }
            else
            {
                unit.SetTarget(null, "Guarding flag");
            }
        }
    }
}
