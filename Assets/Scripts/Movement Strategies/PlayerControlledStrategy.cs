using UnityEngine;

public class PlayerControlledStrategy : IMovementStrategy
{
    private Unit currentTarget;

    public PlayerControlledStrategy()
    {
        UnitEvents.OnEnemyUnitClicked += HandleEnemyClicked;
    }

    public void Dispose()
    {
        UnitEvents.OnEnemyUnitClicked -= HandleEnemyClicked;
    }

    private void HandleEnemyClicked(Unit enemy)
    {
        currentTarget = enemy;
    }

    public void Move(Unit unit)
    {
        if (currentTarget == null)
            return;

        if (unit.Target != currentTarget)
            unit.SetTarget(currentTarget);

        Vector3 dir = (currentTarget.transform.position - unit.transform.position).normalized;
        unit.transform.position += dir * unit.MoveSpeed * Time.deltaTime;
    }
}
