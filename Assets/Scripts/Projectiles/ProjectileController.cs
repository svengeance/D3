using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [field: SerializeField]
    public ProjectileData ProjectileData { get; private set; }

    [field: SerializeField]
    public ProjectileMovementBehavior MovementBehavior { get; private set; }

    [field: SerializeField]
    public ProjectileHitboxBehavior HitboxBehavior { get; private set; }

    private void Awake()
        => HitboxBehavior.OnCollision.AddListener(OnCollision);

    private void Start()
    {
        foreach (var behavior in ProjectileData.ProjectileBehaviors)
            ((IProjectileBehavior)behavior).OnSpawn(this);
    }

    private void OnCollision(GameObject target, Vector2 velocity)
    {
        foreach (var behavior in ProjectileData.ProjectileBehaviors)
            ((IProjectileBehavior)behavior).OnCollide(this, target, velocity);
    }
}
