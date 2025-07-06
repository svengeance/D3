using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [field: SerializeField]
    public ProjectileMovementBehavior MovementBehavior { get; private set; }

    [field: SerializeField]
    public ProjectileBehavior[] ProjectileBehaviors { get; private set; }

    private void Start()
    {
        foreach (var behavior in ProjectileBehaviors)
            behavior.OnSpawn(this);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        foreach (var behavior in ProjectileBehaviors)
            behavior.OnCollide(this, other.gameObject, other.relativeVelocity);
    }
}
