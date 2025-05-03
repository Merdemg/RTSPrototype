using UnityEngine;

public enum Faction
{
    Player,
    Enemy,
    Neutral
}

public class Unit : MonoBehaviour
{
    public UnitStats Stats => stats;
    public UnitType Type => stats.unitType;
    public int PointValue => stats.pointValue;
    public Faction Faction => faction;

    private UnitStats stats;
    private Faction faction;

    private int currentHealth;
    private int maxHealth;
    private float effectiveMoveSpeed;

    public float MoveSpeed => effectiveMoveSpeed;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    private IMovementStrategy movementStrategy;

    private GridManager gridManager;

    private void Update()
    {
        movementStrategy?.Move(this);
    }

    public void Init(UnitStats stats, Faction faction, IMovementStrategy strategy, GridManager gridManager)
    {
        this.stats = stats;
        this.faction = faction;
        movementStrategy = strategy;
        this.gridManager = gridManager;

        maxHealth = Mathf.CeilToInt(stats.maxHealth);
        currentHealth = maxHealth;
        effectiveMoveSpeed = stats.moveSpeed;
    }

    public void SetMovementStrategy(IMovementStrategy strategy)
    {
        movementStrategy = strategy;
    }

    /// <summary>
    /// Only meant to be used in the factory, would fully heal the unit if used for a 'mid game upgrade'
    /// </summary>
    public void ApplyStatModifiers(float speedMultiplier, float healthMultiplier)
    {
        effectiveMoveSpeed = stats.moveSpeed * speedMultiplier;
        maxHealth = Mathf.CeilToInt(stats.maxHealth * healthMultiplier);
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
            Die();
    }

    protected virtual void Die()
    {
        if (gridManager != null)
            gridManager.UnregisterUnit(this);

        Destroy(gameObject);
    }
}
