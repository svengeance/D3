using UnityEngine;

public class DespawnerController : MonoBehaviour
{
    public BoxCollider2D Collider { get; private set; }

    private void Awake()
        => Collider = GetComponent<BoxCollider2D>();
}
