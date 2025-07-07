using UnityEngine;

public abstract class ProjectileBehavior : MonoBehaviour
{
    public virtual void OnEnemyCollide(ProjectileController projectile, EnemyController target, Vector2 relativeVelocity) { }

    public virtual void OnWallCollide(ProjectileController projectile, GameObject target, Vector2 relativeVelocity) { }

    public virtual void OnProjectileCollide(ProjectileController projectile, ProjectileController target, Vector2 relativeVelocity) { }

    public virtual void OnSpawn(ProjectileController projectile) { }
}
