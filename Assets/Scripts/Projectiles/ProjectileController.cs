using System;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [field: SerializeField]
    public ProjectileMovementBehavior Movement { get; private set; }

    [field: SerializeField]
    public ProjectileBehavior[] Behaviors { get; private set; }

    public bool IsDespawning { get; private set; }

    private GameObject LastCollidedObject { get; set; }

    private void Start()
    {
        foreach (var behavior in Behaviors)
            behavior.OnSpawn(this);
    }

    private void FixedUpdate()
    {
        foreach (var behavior in Behaviors)
        {
            if (IsDespawning)
                return;

            behavior.OnFixedUpdate(this);
        }
    }

    public void Despawn()
    {
        if (IsDespawning)
            return;

        IsDespawning = true;

        // Future: trigger a "fizzle out" effect here before destroying.
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsDespawning || LastCollidedObject == collision.gameObject)
            return;

        LastCollidedObject = collision.gameObject;

        Action<ProjectileBehavior> invokeEvent = collision.gameObject.layer switch
        {
            Layers.Terrain => b => b.OnWallCollide(this, collision.gameObject, collision),
            Layers.Enemy when collision.gameObject.TryGetComponent<EnemyController>(out var enemy) => b => b.OnEnemyCollide(this, enemy, collision),
            Layers.Projectile when collision.gameObject.TryGetComponent<ProjectileController>(out var projectile) => b => b.OnProjectileCollide(this, projectile, collision),
            _ => _ => { }
        };

        foreach (var behavior in Behaviors)
            invokeEvent(behavior);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (LastCollidedObject == collision.gameObject)
            LastCollidedObject = null;
    }
}
