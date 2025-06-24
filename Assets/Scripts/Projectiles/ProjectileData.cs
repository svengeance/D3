using UnityEngine;

public class ProjectileData : MonoBehaviour
{
    [field: SerializeField]
    public ProjectileType Type { get; private set; } = ProjectileType.Unknown;

    [field: SerializeField]
    public string Name { get; private set; } = "Unknown";

    [field: SerializeField]
    public float Weight { get; private set; } = 10f;

    [field: SerializeField]
    public float Damage { get; private set; } = 5f;

    public IProjectileOnCollideBehavior[] OnHitBehaviors { get; private set; }
}

public enum ProjectileType
{
    Unknown,
    Basic
}
