using System;
using UnityEngine;

public class DamageEnemyBehavior : MonoBehaviour, IProjectileOnCollideBehavior
{
    [field: SerializeField]
    public int Damage { get; private set; } = 10;

    public void OnCollide(ProjectileController projectile, GameObject target)
        => throw new NotImplementedException();
}
