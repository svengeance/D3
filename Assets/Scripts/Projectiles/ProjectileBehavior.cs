using UnityEngine;

public abstract class ProjectileBehavior : MonoBehaviour
{
    public virtual void OnSpawn(ProjectileController projectile) { }

    public virtual void OnFixedUpdate(ProjectileController projectile) { }

    public virtual void OnEnemyCollide(ProjectileController projectile, EnemyController target, Collision2D collision) { }

    public virtual void OnWallCollide(ProjectileController projectile, GameObject target, Collision2D collision) { }

    public virtual void OnProjectileCollide(ProjectileController projectile, ProjectileController target, Collision2D collision) { }
}
