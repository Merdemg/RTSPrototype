using UnityEngine;

public class MoveToFlagStrategy : IMovementStrategy
{
    private Transform flagTransform;

    public MoveToFlagStrategy(Transform flag)
    {
        flagTransform = flag;
    }

    public void Move(Unit unit)
    {
        Vector3 dir = (flagTransform.position - unit.transform.position).normalized;
        unit.transform.position += dir * unit.MoveSpeed * Time.deltaTime;
    }
}
