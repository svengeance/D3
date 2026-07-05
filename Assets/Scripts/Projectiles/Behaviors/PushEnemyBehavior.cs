using UnityEngine;

public class PushEnemyBehavior : ProjectileBehavior
{
    [field: SerializeField]
    public float ForceMultiplier { get; private set; } = 5f;

    [field: SerializeField]
    public float MaxPushForce { get; private set; } = 10f;

    [field: SerializeField]
    public float GlancingHitMultiplier { get; private set; } = 0.35f;

    public override void OnEnemyCollide(ProjectileController projectile, EnemyController target, Collision2D collision)
    {
        if (target == null)
            return;

        var impactVelocity = projectile.Movement.VelocityLastFrame;
        if (impactVelocity.sqrMagnitude <= 0.0001f)
            impactVelocity = projectile.Movement.Rigidbody.linearVelocity;

        var direction = impactVelocity.sqrMagnitude > 0.0001f
            ? impactVelocity.normalized
            : ((Vector2)target.transform.position - (Vector2)projectile.transform.position).normalized;

        var speed = Mathf.Max(impactVelocity.magnitude, collision.relativeVelocity.magnitude);
        var contactPoint = collision.contactCount > 0 ? collision.GetContact(0).point : (Vector2)target.transform.position;
        var alignment = collision.contactCount > 0
            ? Mathf.Abs(Vector2.Dot(direction, -collision.GetContact(0).normal))
            : 1f;
        var impactStrength = Mathf.Lerp(GlancingHitMultiplier, 1f, alignment);
        var pushImpulse = Mathf.Min(speed * ForceMultiplier * impactStrength, MaxPushForce);

        // Apply the knockback as a physics impulse at the actual contact point so
        // the engine produces both the shove and the correct spin for off-centre hits.
        target.ApplyImpact(direction * pushImpulse, contactPoint);
    }
}
