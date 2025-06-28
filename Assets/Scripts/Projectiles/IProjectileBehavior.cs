using UnityEngine;

public interface IProjectileBehavior
{
    public void OnCollide(ProjectileController projectile, GameObject target) { }

    public void OnSpawn(ProjectileController projectile) { }
}
