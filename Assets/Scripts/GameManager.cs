using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Transform flagTransform;
    [SerializeField] private UnitFactory unitFactory;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Transform[] spawnAnchors;

    [Header("Game Settings")]
    [SerializeField] private GameSettings settings;

    private void Start()
    {
        SpawnPlayerDefenders();
        SpawnEnemies();
    }

    private void SpawnPlayerDefenders()
    {
        Vector3 flagPos = flagTransform.position;
        float spacing = settings.defenderSpacing;
        int count = settings.defenderCount;

        for (int i = 0; i < count; i++)
        {
            float offsetX = (i - (count - 1) / 2f) * spacing;
            Vector3 offset = new Vector3(offsetX, 0, -1f);
            Vector3 position = flagPos + offset;

            IMovementStrategy strategy = GetAIMovementStrategy();
            Unit unit = unitFactory.SpawnUnit(UnitType.AntDefender, position, strategy);
            gridManager.RegisterUnit(unit);
        }
    }

    private void SpawnEnemies()
    {
        foreach (var entry in settings.enemySpawnList)
        {
            for (int i = 0; i < entry.count; i++)
            {
                Vector3 spawnPos = GetRandomSpawnPosition();
                IMovementStrategy strategy = new MoveToFlagStrategy(flagTransform);

                Unit unit = unitFactory.SpawnUnit(entry.type, spawnPos, strategy);
                unit.ApplyStatModifiers(settings.enemySpeedMultiplier, settings.enemyHealthMultiplier);
                gridManager.RegisterUnit(unit);
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        if (spawnAnchors == null || spawnAnchors.Length == 0)
        {
            Debug.LogWarning("No spawn anchors assigned.");
            return Vector3.zero;
        }

        Transform anchor = spawnAnchors[Random.Range(0, spawnAnchors.Length)];
        Vector2 offset2D = Random.insideUnitCircle * settings.spawnRadius;
        Vector3 offset = new Vector3(offset2D.x, 0f, offset2D.y);

        return anchor.position + offset;
    }

    private IMovementStrategy GetAIMovementStrategy()
    {
        return settings.aiType switch
        {
            AIType.ClosestEnemy => new ClosestEnemyStrategy(gridManager),
            AIType.ThreatScoring => new ThreatScoringStrategy(gridManager),
            AIType.None => new IdleStrategy(),
            _ => new IdleStrategy()
        };
    }
}
