using UnityEngine;

public class FlagZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var unit = other.GetComponentInParent<Unit>();
        if (unit != null && unit.Faction == Faction.Enemy)
        {
            GameStateManager.Instance.TriggerLoss();
        }
    }
}
