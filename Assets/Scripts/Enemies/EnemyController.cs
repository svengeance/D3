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

    public UnityEvent<Vector2> OnCollide { get; set; } = new();

    public UnityEvent<float> OnTakeDamage { get; set; } = new();

    public float CurrentHealth { get; set; }

    private void Awake()
        => CurrentHealth = Data.StartingHealth;

    private void Start()
    {
        SpriteRenderer.sprite = Data.Sprite;

        OnCollide.AddListener(MovementBehavior.OnCollide);
        OnTakeDamage.AddListener(Damage);
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
