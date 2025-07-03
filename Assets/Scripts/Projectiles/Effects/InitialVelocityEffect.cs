using UnityEngine;

public class InitialVelocityEffect : MonoBehaviour, IProjectileBehavior
{
    [field: SerializeField]
    public Vector2 Velocity { get; private set; }

    public void OnSpawn(ProjectileController projectile)
        => projectile.MovementBehavior.Rigidbody.AddForce(Velocity, ForceMode2D.Impulse);
}
