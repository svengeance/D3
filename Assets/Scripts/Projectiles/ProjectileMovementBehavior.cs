using UnityEngine;

public class ProjectileMovementBehavior : MonoBehaviour
{
    [field: SerializeField]
    public ProjectileController Projectile { get; private set; }

    [field: SerializeField]
    public Rigidbody2D Rigidbody { get; private set; }

    public Vector2 VelocityLastFrame { get; private set; }

    private void FixedUpdate()
        => VelocityLastFrame = Rigidbody.linearVelocity;
}
