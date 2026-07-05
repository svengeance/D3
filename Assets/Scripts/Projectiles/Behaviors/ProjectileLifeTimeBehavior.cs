using UnityEngine;

// Despawns a projectile once any configured limit is reached. Each limit is
// independently optional (0 = disabled), so a projectile can fade out from a
// lifetime, a bounce budget, dropping too slow, or exceeding a max speed.
public class ProjectileLifeTimeBehavior : ProjectileBehavior
{
    [field: SerializeField]
    public float MaxLifetimeMs { get; private set; } = 0f;

    // Number of terrain bounces before despawning. 0 = disabled.
    [field: SerializeField]
    public int MaxBounces { get; private set; } = 0;

    // Despawn once the projectile slows below this speed (its "fizzle out" point). 0 = disabled.
    [field: SerializeField]
    public float MinVelocity { get; private set; } = 0.75f;

    // Despawn once the projectile exceeds this speed. 0 = disabled.
    [field: SerializeField]
    public float MaxVelocity { get; private set; } = 0f;

    private float _ageMs;
    private int _bounceCount;
    private bool _hasMoved;

    public override void OnFixedUpdate(ProjectileController projectile)
    {
        _ageMs += Time.fixedDeltaTime * 1000f;
        if (MaxLifetimeMs > 0f && _ageMs >= MaxLifetimeMs)
        {
            projectile.Despawn();
            return;
        }

        var speed = projectile.Movement.Rigidbody.linearVelocity.magnitude;

        // Wait until the projectile has actually launched before the min-velocity
        // check can fire, otherwise it would despawn on the first pre-launch frame.
        if (!_hasMoved)
        {
            if (speed >= MinVelocity)
                _hasMoved = true;
            else
                return;
        }

        if (MinVelocity > 0f && speed < MinVelocity)
        {
            projectile.Despawn();
            return;
        }

        if (MaxVelocity > 0f && speed > MaxVelocity)
            projectile.Despawn();
    }

    public override void OnWallCollide(ProjectileController projectile, GameObject target, Collision2D collision)
    {
        if (MaxBounces <= 0)
            return;

        _bounceCount++;
        if (_bounceCount >= MaxBounces)
            projectile.Despawn();
    }
}
