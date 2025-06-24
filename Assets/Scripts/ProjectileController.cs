using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [field: SerializeField]
    public ProjectileData ProjectileData { get; private set; }

    public void Initialize(ProjectileData data)
        => ProjectileData = data;
}
