using UnityEngine;
using UnityEngine.Events;

public class ProjectileHitboxBehavior : MonoBehaviour
{
    public UnityEvent<GameObject, Vector2> OnCollision { get; } = new();

    private void OnCollisionEnter2D(Collision2D other)
        => OnCollision.Invoke(other.gameObject, other.relativeVelocity);
}
