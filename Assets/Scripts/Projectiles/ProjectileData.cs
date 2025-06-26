using UnityEngine;

public class ProjectileData : MonoBehaviour
{
    [field: SerializeField]
    public string Name { get; private set; } = "Unknown";

    [field: SerializeField]
    public MonoBehaviour[] ProjectileBehaviors { get; private set; }
}
