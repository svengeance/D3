using UnityEngine;

public class DespawnerHitboxBehavior : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<EnemyHitboxBehavior>(out var enemyHitbox))
            return;

        enemyHitbox.Enemy.Die();
    }
}
