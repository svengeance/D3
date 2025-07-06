using System.Collections.Generic;
using UnityEngine;

public class PushEnemyEffect : ProjectileBehavior
{
    [field: SerializeField]
    public float ForceMultiplier { get; private set; } = 5f;

    [field: SerializeField]
    public float MaxPushForce { get; private set; } = 10f;

    private HashSet<GameObject> EncounteredEnemies { get; } = new();

    public override void OnCollide(ProjectileController projectile, GameObject target, Vector2 relativeVelocity)
    {
        if (EncounteredEnemies.Contains(target))
            return;

        EncounteredEnemies.Clear();

        if (target.TryGetComponent<EnemyController>(out var enemyController))
        {
            EncounteredEnemies.Add(target);

            var direction = (enemyController.transform.position - projectile.transform.position).normalized;
            var velocity = relativeVelocity.magnitude;
            var pushForce = Mathf.Min(velocity * ForceMultiplier, MaxPushForce);

            enemyController.OnCollide.Invoke(direction * pushForce);
        }
    }
}
