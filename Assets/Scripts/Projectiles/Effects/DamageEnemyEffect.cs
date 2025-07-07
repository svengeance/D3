using UnityEngine;

public class DamageEnemyEffect : ProjectileBehavior
{
    [field: SerializeField]
    public int Damage { get; private set; } = 10;

    public override void OnEnemyCollide(ProjectileController projectile, EnemyController target, Vector2 relativeVelocity)
    {
        if (target.TryGetComponent<EnemyController>(out var enemyController))
            enemyController.OnTakeDamage.Invoke(Damage);
    }
}
