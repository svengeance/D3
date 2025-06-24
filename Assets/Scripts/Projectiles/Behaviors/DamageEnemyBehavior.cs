using System;
using UnityEngine;

public class DamageEnemyBehavior : MonoBehaviour, IProjectileOnCollideBehavior
{
    public void OnCollide(ProjectileController projectile, GameObject target)
        => throw new NotImplementedException();
}
