using UnityEngine;

public class EnemyCollisionVfxController : MonoBehaviour
{
    [field: SerializeField]
    public Material EnemyCollisionMaterial { get; set; }

    [field: SerializeField]
    public Material WallCollisionMaterial { get; set; }

    [field: SerializeField]
    public ParticleSystem ParticleSystemPrefab { get; set; }

    public void PlayVfx(CollisionType collisionType, Vector2 collisionPoint)
    {
        var particleSystem = Instantiate(ParticleSystemPrefab, collisionPoint, Quaternion.identity);
        var renderer = particleSystem.GetComponent<ParticleSystemRenderer>();

        renderer.sharedMaterial = collisionType switch
        {
            CollisionType.EnemyEnemy => EnemyCollisionMaterial,
            CollisionType.EnemyTerrain => WallCollisionMaterial,
            _ => renderer.sharedMaterial
        };

        Destroy(particleSystem.gameObject, particleSystem.main.duration + particleSystem.main.startLifetime.constantMax);
    }
}
