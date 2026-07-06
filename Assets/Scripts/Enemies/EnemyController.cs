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
    private float EnemyDamagePerMassImpactSpeed { get; set; } = 0.5f;

    [field: SerializeField]
    private float MinEnemyImpactSpeed { get; set; } = 3f;

    [field: SerializeField]
    private float ImpactSpinMultiplier { get; set; } = 72f;

    [field: SerializeField]
    private float MaxImpactSpin { get; set; } = 1440f;

    [field: SerializeField]
    private float MaxSpinImpulse { get; set; } = 12f; // caps the impulse used for torque so tuning push force (knockback) doesn't also tune spin

    // x = how close the hit is to a corner (0 = dead center of an edge, 1 = exact corner); y = spin multiplier at that point
    [field: SerializeField]
    private AnimationCurve SpinCornerMultiplier { get; set; } = new(
        new Keyframe(0f, 0.5f),
        new Keyframe(0.20f, 0.5f),
        new Keyframe(0.65f, 0.85f),
        new Keyframe(1f, 1.25f));

    private BoxCollider2D VehicleCollider { get; set; }

    public UnityEvent<float> OnTakeDamage { get; set; } = new();

    public float CurrentHealth { get; set; }

    private void Awake()
    {
        CurrentHealth = Data.StartingHealth;
        VehicleCollider = MovementBehavior.RigidBody.GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        SpriteRenderer.sprite = Data.Sprite;

        OnTakeDamage.AddListener(Damage);
    }

    // EnemyMovementBehavior splits goal-seeking drive from impact state, so impacts are
    // fed in as external velocity/spin that decay over time instead of being overwritten
    // by the tank AI on the next FixedUpdate.
    public void ApplyImpact(Vector2 impulse, Vector2 worldPoint)
    {
        var rigidBody = MovementBehavior.RigidBody;

        MovementBehavior.ApplyExternalVelocity(impulse / rigidBody.mass);

        var contactOffset = worldPoint - rigidBody.worldCenterOfMass;
        var spinImpulse = Vector2.ClampMagnitude(impulse, MaxSpinImpulse);
        var cornerness = HitCornerness(contactOffset);
        var spin = (contactOffset.x * spinImpulse.y - contactOffset.y * spinImpulse.x) * ImpactSpinMultiplier * SpinCornerMultiplier.Evaluate(cornerness);
        MovementBehavior.ApplyExternalSpin(Mathf.Clamp(spin, -MaxImpactSpin, MaxImpactSpin));
    }

    // 0 = hit landed at the center of an edge, 1 = hit landed on a corner. contactOffset is
    // rotated into the collider's local space first since the tank can be facing any direction.
    private float HitCornerness(Vector2 contactOffset)
    {
        var localOffset = MovementBehavior.RigidBody.transform.InverseTransformVector(contactOffset);
        var halfExtents = VehicleCollider.size * 0.5f;

        return Mathf.Clamp01(Mathf.Min(Mathf.Abs(localOffset.x) / halfExtents.x, Mathf.Abs(localOffset.y) / halfExtents.y));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case Layers.Terrain:
                HandleTerrainCollision(collision);
                return;
            case Layers.Enemy:
                HandleEnemyCollision(collision);
                return;
        }
    }

    private void HandleTerrainCollision(Collision2D collision)
    {
        var impactSpeed = collision.relativeVelocity.magnitude;
        if (impactSpeed < MinTerrainImpactSpeed)
            return;

        OnTakeDamage.Invoke(impactSpeed * TerrainDamagePerImpactSpeed);
    }

    private void HandleEnemyCollision(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent<EnemyController>(out var otherEnemy))
            return;

        var impactSpeed = collision.relativeVelocity.magnitude;
        if (impactSpeed < MinEnemyImpactSpeed)
            return;

        if (collision.rigidbody == null)
            return;

        OnTakeDamage.Invoke(collision.rigidbody.mass * impactSpeed * EnemyDamagePerMassImpactSpeed);
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
