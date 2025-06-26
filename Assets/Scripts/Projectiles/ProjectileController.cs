using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [field: SerializeField]
    public ProjectileData ProjectileData { get; private set; }

    [field: SerializeField]
    public ProjectileMovementBehavior MovementBehavior { get; private set; }

    public void Initialize() { }
}
