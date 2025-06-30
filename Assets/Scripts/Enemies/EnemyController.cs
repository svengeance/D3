using UnityEngine;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    [field: SerializeField]
    public EnemyData EnemyData { get; private set; }

    [field: SerializeField]
    private EnemyMovementBehavior MovementBehavior { get; set; }

    public UnityEvent<Vector2> OnCollide { get; } = new();

    public void Die()
        => Destroy(gameObject);
}
