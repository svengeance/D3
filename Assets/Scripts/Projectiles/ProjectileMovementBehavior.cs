using UnityEngine;

public class ProjectileMovementBehavior : MonoBehaviour
{
    [field: SerializeField]
    public ProjectileController Projectile { get; private set; }

    [field: SerializeField]
    public Rigidbody2D Rigidbody { get; private set; }

    public void ApplyForce(Vector2 force)
        => Rigidbody.AddForce(force, ForceMode2D.Force);
}
