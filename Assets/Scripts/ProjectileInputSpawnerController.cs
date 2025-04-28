using UnityEngine;

public class ProjectileInputSpawnerController : MonoBehaviour
{
    [field: SerializeField]
    public float InputRadius { get; private set; }

    [field: SerializeField]
    public ProjectileSpawnerController ProjectileSpawner { get; private set; }

    [field: SerializeField]
    public PlayerController Player { get; private set; }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, InputRadius);
    }
}
