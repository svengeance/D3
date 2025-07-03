using System;
using UnityEngine;

public class DamageEnemyEffect : MonoBehaviour, IProjectileBehavior
{
    [field: SerializeField]
    public int Damage { get; private set; } = 10;

    public void OnCollide(ProjectileController projectile, GameObject target)
        => throw new NotImplementedException();
}
