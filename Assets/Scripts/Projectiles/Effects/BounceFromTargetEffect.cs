using System;
using UnityEngine;

public class BounceFromTargetEffect : MonoBehaviour, IProjectileBehavior
{
    [field: SerializeField]
    public float BounceMultiplier { get; private set; } = 1.0f;

    [field: SerializeField]
    public float AngleVariance { get; private set; }

    public void OnCollide(ProjectileController projectile, GameObject target)
        => throw new NotImplementedException();
}
