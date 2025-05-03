using UnityEngine;

public enum Faction
{
    Player,
    Enemy,
    Neutral
}

public class Unit : MonoBehaviour
{
    [SerializeField] private UnitStats stats;
    [SerializeField] private Faction faction;

    public UnitStats Stats => stats;
    public UnitType Type => stats.unitType;
    public int PointValue => stats.pointValue;
    public Faction Faction => faction;

    private int currentHealth;
    private int maxHealth;
    private float effectiveMoveSpeed;

    public float MoveSpeed => effectiveMoveSpeed;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    private IMovementStrategy movementStrategy;

    private void Update()
    {
        movementStrategy?.Move(this);
    }
    public void Init(UnitStats stats, Faction faction, IMovementStrategy strategy)
    {
        this.stats = stats;
        this.faction = faction;
        movementStrategy = strategy;

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
        // For now: destroy unit
        Destroy(gameObject);
    }
}
