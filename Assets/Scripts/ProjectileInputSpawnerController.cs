using UnityEngine;

public class ProjectileInputSpawnerController : MonoBehaviour
{
    [field: SerializeField]
    public ProjectileSpawnerController ProjectileSpawner { get; private set; }

    [field: SerializeField]
    public PlayerController Player { get; private set; }

    [field: SerializeField]
    public DragIndicatorController DragIndicator { get; private set; }

    public void SpawnProjectile(Vector2 spawnPosition, Vector2 direction)
        => ProjectileSpawner.SpawnProjectile(PlayerManager.Instance.SelectedProjectileData, spawnPosition, direction);
}
