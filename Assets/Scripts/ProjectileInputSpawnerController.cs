using UnityEngine;

public class ProjectileInputSpawnerController : MonoBehaviour
{
    [field: SerializeField]
    public ProjectileSpawnerController ProjectileSpawner { get; private set; }

    [field: SerializeField]
    public PlayerController Player { get; private set; }

    [field: SerializeField]
    public DragIndicatorController DragIndicator { get; private set; }
}
