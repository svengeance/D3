using UnityEngine;

public class ExplodeOnContactEffect : MonoBehaviour, IProjectileBehavior
{
    [field: SerializeField]
    public float ForceMultiplier { get; private set; } = 1f;

    [field: SerializeField]
    public float MaxPushForce { get; private set; } = 5f;

    public void OnCollide(ProjectileController projectile, GameObject target) { }
}
