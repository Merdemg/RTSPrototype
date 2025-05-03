using UnityEngine;

[CreateAssetMenu(fileName = "UnitConfig", menuName = "Scriptable Objects/Unit Config")]
public class UnitConfig : ScriptableObject
{
    public UnitType unitType;
    public UnitStats stats;
    public GameObject prefab;
}
