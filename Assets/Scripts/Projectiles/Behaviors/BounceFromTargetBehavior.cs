using System;
using UnityEngine;

public class BounceFromTargetBehavior : MonoBehaviour, IProjectileOnCollideBehavior
{
    public void OnCollide(ProjectileController projectile, GameObject target)
        => throw new NotImplementedException();
}
