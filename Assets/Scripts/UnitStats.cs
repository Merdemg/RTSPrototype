using UnityEngine;

public enum UnitType
{
    AntDefender,
    Aphid,
    Ladybug,
    Beetle
}

[CreateAssetMenu(fileName = "UnitStats", menuName = "Scriptable Objects/Unit Stats")]
public class UnitStats : ScriptableObject
{
    public string unitName;
    public float moveSpeed;
    public int maxHealth;
    public int damage;
    public int pointValue;
    public UnitType unitType;
}
