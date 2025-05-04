using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene References")]
    [SerializeField] private Transform flagTransform;
    [SerializeField] private UnitFactory unitFactory;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Transform[] spawnAnchors;

    [Header("Game Settings")]
    [SerializeField] private GameSettings settingsAsset;

    private GameSettings settingsInstance;
    public GameSettings Settings => settingsInstance;

    public Vector3 FlagPosition => flagTransform.position;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure only one exists
            return;
        }
        Instance = this;

        settingsInstance = Instantiate(settingsAsset);
    }

    public void StartGame()
    {
        SpawnPlayerDefenders();
        SpawnEnemies();
        GameStateManager.Instance.SetState(GameState.Playing);
    }

    public void RestartGame()
    {
        UnitRegistry.Instance.DestroyAll();
        ScoreManager.Instance.ResetStats();
        GameStateManager.Instance.SetState(GameState.MainMenu);
    }

    private void SpawnPlayerDefenders()
    {
        Vector3 flagPos = flagTransform.position;
        float spacing = Settings.defenderSpacing;
        int count = Settings.defenderCount;

        for (int i = 0; i < count; i++)
        {
            float offsetX = (i - (count - 1) / 2f) * spacing;
            Vector3 offset = new Vector3(offsetX, 0, -1f);
            Vector3 position = flagPos + offset;

            IMovementStrategy strategy = GetAIMovementStrategy(i);
            Unit unit = unitFactory.SpawnUnit(UnitType.AntDefender, position, strategy, gridManager);
            gridManager.RegisterUnit(unit);
        }
    }

    private void SpawnEnemies()
    {
        foreach (var entry in Settings.enemySpawnList)
        {
            for (int i = 0; i < entry.count; i++)
            {
                Vector3 spawnPos = GetRandomSpawnPosition();
                IMovementStrategy strategy = new MoveToFlagStrategy(flagTransform);

                Unit unit = unitFactory.SpawnUnit(entry.type, spawnPos, strategy, gridManager);
                unit.ApplyStatModifiers(Settings.enemySpeedMultiplier, Settings.enemyHealthMultiplier);
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
        Vector2 offset2D = Random.insideUnitCircle * Settings.spawnRadius;
        Vector3 offset = new Vector3(offset2D.x, 0f, offset2D.y);

        return anchor.position + offset;
    }

    public void ApplyCurrentDefenderStrategy()
    {
        var defenders = UnitRegistry.Instance.GetDefenders();

        for (int i = 0; i < defenders.Count; i++)
        {
            var unit = defenders[i];

            if (unit.MovementStrategy is System.IDisposable disposable)
                disposable.Dispose();

            unit.SetMovementStrategy(GetAIMovementStrategy(i));
        }
    }

    private IMovementStrategy GetAIMovementStrategy(int index)
    {
        return Settings.aiType switch
        {
            AIType.ClosestEnemy => new ClosestEnemyStrategy(gridManager),
            AIType.ThreatScoring => new ThreatScoringStrategy(gridManager, flagTransform),
            AIType.PlayerControlled => new PlayerControlledStrategy(),
            AIType.ErdemsSpecial => index < (Settings.defenderCount + 1) / 2
                ? new VanguardStrategy(gridManager, UnitRegistry.Instance)
                : new RearguardStrategy(gridManager, flagTransform),
            AIType.None => new IdleStrategy(),
            _ => new IdleStrategy()
        };
    }
}
