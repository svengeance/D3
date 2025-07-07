using UnityEngine;

public class BounceFromTargetEffect : ProjectileEffect
{
    [field: SerializeField]
    public float BounceMultiplier { get; private set; } = 0.2f;

    [field: SerializeField]
    public float AngleVariance { get; private set; }

    public override void OnEnemyCollide(ProjectileController projectile, EnemyController target, Collision2D collision)
    {
        if (collision.contactCount == 0)
            return;

        var projectileRb = projectile.Movement.Rigidbody;
        var priorVelocity = projectile.Movement.VelocityLastFrame;

        var adjustedVelocity = priorVelocity.magnitude * BounceMultiplier;

        projectileRb.linearVelocity = adjustedVelocity * projectileRb.linearVelocity.normalized;
    }
}
