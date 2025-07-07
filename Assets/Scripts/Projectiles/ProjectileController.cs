using System;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [field: SerializeField]
    public ProjectileMovementBehavior MovementBehavior { get; private set; }

    [field: SerializeField]
    public ProjectileBehavior[] ProjectileBehaviors { get; private set; }

    private GameObject LastCollidedObject { get; set; }

    private void Start()
    {
        foreach (var behavior in ProjectileBehaviors)
            behavior.OnSpawn(this);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (LastCollidedObject == other.gameObject)
            return;

        LastCollidedObject = other.gameObject;

        Action<ProjectileBehavior> invokeEvent = other.gameObject.layer switch
        {
            Layers.Terrain => b => b.OnWallCollide(this, other.gameObject, other.relativeVelocity),
            Layers.Enemy => b => b.OnEnemyCollide(this, other.gameObject.GetComponent<EnemyController>(), other.relativeVelocity),
            Layers.Projectile => b => b.OnProjectileCollide(this, other.gameObject.GetComponent<ProjectileController>(), other.relativeVelocity),
            _ => _ => { }
        };

        foreach (var behavior in ProjectileBehaviors)
            invokeEvent(behavior);
    }
}
