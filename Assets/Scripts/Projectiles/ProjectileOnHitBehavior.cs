using UnityEngine;

public interface IProjectileOnCollideBehavior
{
    public void OnCollide(ProjectileController projectile, GameObject target);
}
