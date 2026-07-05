using UnityEngine;

public class InitialVelocityBehavior : ProjectileBehavior
{
    [field: SerializeField]
    public Vector2 Velocity { get; private set; }

    public override void OnSpawn(ProjectileController projectile)
        => projectile.Movement.Rigidbody.AddForce(Velocity, ForceMode2D.Impulse);
}
