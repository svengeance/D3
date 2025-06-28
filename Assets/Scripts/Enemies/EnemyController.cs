using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private EnemyData _enemyData;

    [field: SerializeField]
    public PlayerController Player { get; private set; }

    public void Initialize(PlayerController playerController)
        => Player = playerController;

    public void Die()
        => Destroy(gameObject);
}
