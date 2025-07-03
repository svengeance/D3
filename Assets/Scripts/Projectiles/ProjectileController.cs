using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [field: SerializeField]
    public ProjectileData ProjectileData { get; private set; }

    [field: SerializeField]
    public ProjectileMovementBehavior MovementBehavior { get; private set; }

    private void Start()
    {
        foreach (var behavior in ProjectileData.ProjectileBehaviors)
            ((IProjectileBehavior)behavior).OnSpawn(this);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        foreach (var behavior in ProjectileData.ProjectileBehaviors)
            ((IProjectileBehavior)behavior).OnCollide(this, other.gameObject, other.relativeVelocity);
    }
}
