using UnityEngine;

public class EnemyDespawnerController : MonoBehaviour
{
    public BoxCollider2D Collider { get; private set; }

    private void Awake()
        => Collider = GetComponent<BoxCollider2D>();
}
