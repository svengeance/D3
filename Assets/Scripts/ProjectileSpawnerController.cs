using UnityEngine;

public class ProjectileSpawnerController : MonoBehaviour
{
    [field: SerializeField]
    public ProjectileController Projectile { get; private set; }

    public ProjectileController SpawnProjectile(ProjectileData data, Vector2 position)
    {
        var projectile = Instantiate(Projectile, position, Quaternion.identity);
        projectile.Initialize(data);

        return projectile;
    }
}
