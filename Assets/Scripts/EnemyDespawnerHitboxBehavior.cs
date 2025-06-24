using UnityEngine;

public class EnemyDespawnerHitboxBehavior : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<EnemyHitboxBehavior>(out var enemyHitbox))
            return;

        enemyHitbox.Enemy.Die();
    }
}
