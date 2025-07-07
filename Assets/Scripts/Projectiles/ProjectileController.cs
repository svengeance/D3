using System;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [field: SerializeField]
    public ProjectileMovementBehavior Movement { get; private set; }

    [field: SerializeField]
    public ProjectileEffect[] Effects { get; private set; }

    private GameObject LastCollidedObject { get; set; }

    private void Start()
    {
        foreach (var behavior in Effects)
            behavior.OnSpawn(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (LastCollidedObject == collision.gameObject)
            return;

        LastCollidedObject = collision.gameObject;

        Action<ProjectileEffect> invokeEvent = collision.gameObject.layer switch
        {
            Layers.Terrain => b => b.OnWallCollide(this, collision.gameObject, collision),
            Layers.Enemy => b => b.OnEnemyCollide(this, collision.gameObject.GetComponent<EnemyController>(), collision),
            Layers.Projectile => b => b.OnProjectileCollide(this, collision.gameObject.GetComponent<ProjectileController>(), collision),
            _ => _ => { }
        };

        foreach (var behavior in Effects)
            invokeEvent(behavior);
    }
}
