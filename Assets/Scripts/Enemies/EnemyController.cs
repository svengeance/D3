using UnityEngine;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    [field: SerializeField]
    public EnemyData Data { get; private set; }

    [field: SerializeField]
    private EnemyMovementBehavior MovementBehavior { get; set; }

    [field: SerializeField]
    private SpriteRenderer SpriteRenderer { get; set; }

    [field: SerializeField]
    public EnemyHealthBarBehavior HealthBar { get; private set; }

    [field: SerializeField]
    private float TerrainDamagePerImpactSpeed { get; set; } = 1f;

    [field: SerializeField]
    private float MinTerrainImpactSpeed { get; set; } = 2f;

    [field: SerializeField]
    private float ImpactSpinMultiplier { get; set; } = 6f;

    [field: SerializeField]
    private float MaxImpactSpin { get; set; } = 45f;

    public UnityEvent<float> OnTakeDamage { get; set; } = new();

    public float CurrentHealth { get; set; }

    private void Awake()
        => CurrentHealth = Data.StartingHealth;

    private void Start()
    {
        SpriteRenderer.sprite = Data.Sprite;

        OnTakeDamage.AddListener(Damage);
    }

    // EnemyMovementBehavior overwrites linearVelocity and rotation every FixedUpdate, so
    // physics impulses/torque get erased before they render. Route both the shove and the
    // spin through its decaying offsets instead so they actually persist and ease out.
    public void ApplyImpact(Vector2 impulse, Vector2 worldPoint)
    {
        var rigidBody = MovementBehavior.RigidBody;

        MovementBehavior.OnCollide(impulse / rigidBody.mass);

        var contactOffset = worldPoint - rigidBody.worldCenterOfMass;
        var spin = (contactOffset.x * impulse.y - contactOffset.y * impulse.x) * ImpactSpinMultiplier;
        MovementBehavior.ApplySpin(Mathf.Clamp(spin, -MaxImpactSpin, MaxImpactSpin));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer != Layers.Terrain)
            return;

        var impactSpeed = collision.relativeVelocity.magnitude;
        if (impactSpeed < MinTerrainImpactSpeed)
            return;

        OnTakeDamage.Invoke(impactSpeed * TerrainDamagePerImpactSpeed);
    }

    private void Damage(float amount)
    {
        CurrentHealth -= amount;
        HealthBar.UpdateHealthBar(CurrentHealth, Data.StartingHealth);

        if (CurrentHealth <= 0)
            Die();
    }

    private void Die()
        => Destroy(gameObject);
}
