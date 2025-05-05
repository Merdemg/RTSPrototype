using UnityEngine;
using DG.Tweening;

public enum Faction
{
    Player,
    Enemy,
    Neutral
}

public class Unit : MonoBehaviour
{
    [SerializeField] Renderer unitRenderer;
    Color originalColor;

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

    public IMovementStrategy MovementStrategy => movementStrategy;
    private IMovementStrategy movementStrategy;
    private GridManager gridManager;

    private float attackCooldown = 0f;

    public Unit Target { get; private set; }
    public string TargetReason { get; private set; }

    public bool IsDead { get; private set; } = false;

    Vector3 lastPos = Vector3.zero;

    private void Awake()
    {
        originalColor = unitRenderer.material.color;
    }

    private void Update()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Playing)
            return;

        movementStrategy?.Move(this);
        gridManager?.MoveUnit(this, lastPos);
        lastPos = transform.position;

        if (attackCooldown > 0f)
            attackCooldown -= Time.deltaTime;

        TryAttackTarget();
    }

    public void Init(UnitStats stats, Faction faction, IMovementStrategy strategy, GridManager gridManager)
    {
        this.stats = stats;
        this.faction = faction;
        this.movementStrategy = strategy;
        this.gridManager = gridManager;

        lastPos = transform.position;

        maxHealth = Mathf.CeilToInt(stats.maxHealth);
        currentHealth = maxHealth;
        effectiveMoveSpeed = stats.moveSpeed;

        UnitEvents.RaiseUnitSpawned(this);
    }

    public void SetMovementStrategy(IMovementStrategy strategy)
    {
        movementStrategy = strategy;
    }

    public void SetTarget(Unit target, string reason = "")
    {
        if (Target != target)
        {
            Target = target;
            TargetReason = reason;
        }
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

    private void TryAttackTarget()
    {
        if (attackCooldown > 0f || Target == null || Target.Faction == this.Faction)
            return;

        float sqrDist = (Target.transform.position - transform.position).sqrMagnitude;
        float range = stats.attackRange;

        if (sqrDist <= range * range)
        {
            Target.TakeDamage(stats.damage);
            attackCooldown = stats.attackInterval;
        }
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        currentHealth -= amount;

        transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 1f);
        unitRenderer.material.DOColor(Color.magenta, 0.1f).OnComplete(() => unitRenderer.material.DOColor(originalColor, 0.2f));

        if (currentHealth <= 0)
            Die();
    }

    public void ForceCleanup()
    {
        Die();
    }

    protected virtual void Die() // TODO: Add an object pool?
    {
        if (IsDead) return;

        IsDead = true;

        UnitEvents.RaiseUnitDied(this);

        if (movementStrategy is System.IDisposable disposable)
            disposable.Dispose();

        gridManager?.UnregisterUnit(this);

        var colliders = GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
            col.enabled = false;

        transform
        .DOScale(Vector3.zero, 0.3f)
        .SetEase(Ease.InBack)
        .OnComplete(() => Destroy(gameObject));
    }

    public void HandleClick()
    {
        if (faction == Faction.Enemy)
        {
            UnitEvents.RaiseEnemyUnitClicked(this);
        }
    }
}
