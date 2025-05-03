using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : MonoBehaviour
{
    [Header("All Unit Configs")]
    [SerializeField] private UnitConfig[] allUnitConfigs;

    private Dictionary<UnitType, UnitConfig> configLookup;

    private void Awake()
    {
        configLookup = new Dictionary<UnitType, UnitConfig>();
        foreach (var config in allUnitConfigs)
        {
            if (configLookup.ContainsKey(config.unitType))
                Debug.LogWarning($"Duplicate config for {config.unitType}");
            else
                configLookup[config.unitType] = config;
        }
    }

    public Unit SpawnUnit(UnitType type, Vector3 position, IMovementStrategy movementStrategy)
    {
        if (!configLookup.TryGetValue(type, out var config))
        {
            Debug.LogError($"No UnitConfig found for UnitType: {type}");
            return null;
        }

        GameObject obj = Instantiate(config.prefab, position, Quaternion.identity);
        Unit unit = obj.GetComponent<Unit>();

        if (unit == null)
        {
            Debug.LogError($"Prefab for {type} does not contain a Unit component.");
            return null;
        }

        Faction faction = (type == UnitType.AntDefender) ? Faction.Player : Faction.Enemy;

        unit.Init(config.stats, faction, movementStrategy);

        return unit;
    }
}
