using UnityEngine;

public class EnemyDespawnerController : MonoBehaviour
{
    public BoxCollider2D Collider { get; private set; }

    private void Start()
        => Collider = GetComponent<BoxCollider2D>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<EnemyController>(out var enemy))
            return;

        enemy.OnTakeDamage.Invoke(enemy.CurrentHealth);
    }
}
