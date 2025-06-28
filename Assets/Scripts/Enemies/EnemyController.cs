using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private EnemyData _enemyData;

    public void Die()
        => Destroy(gameObject);
}
