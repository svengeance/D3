using UnityEngine;

public class InitialVelocityEffect : ProjectileBehavior
{
    [field: SerializeField]
    public Vector2 Velocity { get; private set; }

    public override void OnSpawn(ProjectileController projectile)
        => projectile.MovementBehavior.Rigidbody.AddForce(Velocity, ForceMode2D.Impulse);
}
