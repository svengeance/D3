using UnityEngine;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    [field: SerializeField]
    public EnemyData Data { get; private set; }

    [field: SerializeField]
    private EnemyMovementBehavior MovementBehavior { get; set; }

    [field: SerializeField]
    private EnemyCollisionVfxController CollisionVfxController { get; set; }

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

    public UnityEvent<float> OnTakeDamage { get; set; } = new();

    public float CurrentHealth { get; set; }

    private void Awake()
        => CurrentHealth = Data.StartingHealth;

    private void Start()
    {
        SpriteRenderer.sprite = Data.Sprite;

        OnTakeDamage.AddListener(Damage);
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

    public void ApplyImpact(Vector2 impulse, Vector2 worldPoint)
        => MovementBehavior.RigidBody.AddForceAtPosition(impulse, worldPoint, ForceMode2D.Impulse);

    private void HandleTerrainCollision(Collision2D collision)
    {
        var impactSpeed = collision.relativeVelocity.magnitude;
        if (impactSpeed < MinTerrainImpactSpeed)
            return;

        OnTakeDamage.Invoke(impactSpeed * TerrainDamagePerImpactSpeed);
        CollisionVfxController.PlayVfx(CollisionType.EnemyTerrain, collision.GetContact(0).point);
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
        CollisionVfxController.PlayVfx(CollisionType.EnemyEnemy, collision.GetContact(0).point);
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
