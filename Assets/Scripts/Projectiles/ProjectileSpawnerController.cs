using UnityEngine;

public class ProjectileSpawnerController : MonoBehaviour
{
    public ProjectileController SpawnProjectile(ProjectileController projectile, Vector2 position, Vector2 launchForce)
    {
        var projectileObj = Instantiate(projectile, position, Quaternion.identity);

        projectileObj.Movement.Rigidbody.AddForce(launchForce, ForceMode2D.Impulse);

        return projectileObj;
    }
}
