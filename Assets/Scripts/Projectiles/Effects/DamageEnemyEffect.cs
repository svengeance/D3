using UnityEngine;

public class DamageEnemyEffect : ProjectileEffect
{
    [field: SerializeField]
    public int Damage { get; private set; } = 10;

    public override void OnEnemyCollide(ProjectileController projectile, EnemyController target, Collision2D collision)
    {
        if (target.TryGetComponent<EnemyController>(out var enemyController))
            enemyController.OnTakeDamage.Invoke(Damage);
    }
}
