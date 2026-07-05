using UnityEngine;

public class BounceFromTargetBehavior : ProjectileBehavior
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
        if (priorVelocity.sqrMagnitude <= 0.0001f)
            priorVelocity = projectileRb.linearVelocity;

        if (priorVelocity.sqrMagnitude <= 0.0001f)
            return;

        var normal = collision.GetContact(0).normal;
        if (Vector2.Dot(priorVelocity, normal) > 0f)
            normal = -normal;

        var reflectedDirection = Vector2.Reflect(priorVelocity.normalized, normal).normalized;

        if (AngleVariance > 0f)
        {
            var deviation = Random.Range(-AngleVariance, AngleVariance);
            reflectedDirection = (Vector2)(Quaternion.AngleAxis(deviation, Vector3.forward) * reflectedDirection);
        }

        var adjustedSpeed = priorVelocity.magnitude * BounceMultiplier;

        projectileRb.linearVelocity = reflectedDirection * adjustedSpeed;
    }
}
