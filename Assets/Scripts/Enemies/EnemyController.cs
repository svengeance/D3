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
    private float ImpactAngularImpulseMultiplier { get; set; } = 0.65f;

    [field: SerializeField]
    private float MaxImpactAngularImpulse { get; set; } = 5f;

    [field: SerializeField]
    private float MaxImpactAngularVelocity { get; set; } = 65f;

    public UnityEvent<float> OnTakeDamage { get; set; } = new();

    public float CurrentHealth { get; set; }

    private void Awake()
        => CurrentHealth = Data.StartingHealth;

    private void Start()
    {
        SpriteRenderer.sprite = Data.Sprite;

        OnTakeDamage.AddListener(Damage);
    }

    // Applies a knockback impulse at a world contact point so the hit imparts
    // physically-correct linear motion and spin via the physics engine.
    public void ApplyImpact(Vector2 impulse, Vector2 worldPoint)
    {
        var rigidBody = MovementBehavior.RigidBody;

        rigidBody.AddForceAtPosition(impulse, worldPoint, ForceMode2D.Impulse);

        var contactOffset = worldPoint - rigidBody.worldCenterOfMass;
        var angularImpulse = Vector3.Cross(contactOffset, impulse).z * ImpactAngularImpulseMultiplier;
        angularImpulse = Mathf.Clamp(angularImpulse, -MaxImpactAngularImpulse, MaxImpactAngularImpulse);

        rigidBody.AddTorque(angularImpulse, ForceMode2D.Impulse);
        rigidBody.angularVelocity = Mathf.Clamp(rigidBody.angularVelocity, -MaxImpactAngularVelocity, MaxImpactAngularVelocity);
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
