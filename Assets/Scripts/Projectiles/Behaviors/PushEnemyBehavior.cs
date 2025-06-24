using UnityEngine;

public class PushEnemyBehavior : MonoBehaviour, IProjectileOnCollideBehavior
{
    [field: SerializeField]
    public float ForceMultiplier { get; private set; } = 1f;

    [field: SerializeField]
    public float MaxPushForce { get; private set; } = 5f;

    public void OnCollide(ProjectileController projectile, GameObject target) { }
}
