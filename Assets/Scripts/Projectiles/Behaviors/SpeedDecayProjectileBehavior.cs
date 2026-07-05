using UnityEngine;

// Gradually changes a projectile's speed over time while preserving its direction.
// Positive DecayRate slows the projectile down; a negative DecayRate accelerates it.
public class SpeedDecayProjectileBehavior : ProjectileBehavior
{
    [field: SerializeField]
    public float DecayRate { get; private set; } = 0.6f;

    // Upper speed clamp so a negative DecayRate (acceleration) can't run away. 0 = uncapped.
    [field: SerializeField]
    public float MaxSpeed { get; private set; } = 40f;

    public override void OnFixedUpdate(ProjectileController projectile)
    {
        var rb = projectile.Movement.Rigidbody;
        var velocity = rb.linearVelocity;
        var speed = velocity.magnitude;
        if (speed <= 0.0001f)
            return;

        var newSpeed = speed - DecayRate * Time.fixedDeltaTime;
        if (newSpeed <= 0f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (MaxSpeed > 0f)
            newSpeed = Mathf.Min(newSpeed, MaxSpeed);

        rb.linearVelocity = velocity * (newSpeed / speed);
    }
}
