using UnityEngine;

public interface IProjectileBehavior
{
    public void OnCollide(ProjectileController projectile, GameObject target, Vector2 relativeVelocity) { }

    public void OnSpawn(ProjectileController projectile) { }
}
