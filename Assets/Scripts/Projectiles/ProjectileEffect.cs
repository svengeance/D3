using UnityEngine;

public abstract class ProjectileEffect : MonoBehaviour
{
    public virtual void OnEnemyCollide(ProjectileController projectile, EnemyController target, Collision2D collision) { }

    public virtual void OnWallCollide(ProjectileController projectile, GameObject target, Collision2D collision) { }

    public virtual void OnProjectileCollide(ProjectileController projectile, ProjectileController target, Collision2D collision) { }

    public virtual void OnSpawn(ProjectileController projectile) { }
}
