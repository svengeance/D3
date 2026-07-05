using UnityEngine;

public class DamageEnemyBehavior : ProjectileBehavior
{
    [field: SerializeField]
    public int Damage { get; private set; } = 10;

    public override void OnEnemyCollide(ProjectileController projectile, EnemyController target, Collision2D collision)
    {
        if (target != null)
            target.OnTakeDamage.Invoke(Damage);
    }
}
