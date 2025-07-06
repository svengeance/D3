using UnityEngine;

public abstract class ProjectileBehavior : MonoBehaviour
{
    public virtual void OnCollide(ProjectileController projectile, GameObject target, Vector2 relativeVelocity) { }

    public virtual void OnSpawn(ProjectileController projectile) { }
}
