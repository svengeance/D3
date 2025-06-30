using UnityEngine;

public class PushEnemyBehavior : MonoBehaviour, IProjectileBehavior
{
    [field: SerializeField]
    public float ForceMultiplier { get; private set; } = 1f;

    [field: SerializeField]
    public float MaxPushForce { get; private set; } = 5f;

    public void OnCollide(ProjectileController projectile, GameObject target, Vector2 relativeVelocity)
    {
        if (target.TryGetComponent<EnemyController>(out var enemyController))
        {
            var direction = (projectile.transform.position - enemyController.transform.position).normalized;
            var velocity = relativeVelocity.magnitude;
            var pushForce = Mathf.Min(velocity * ForceMultiplier, MaxPushForce);

            enemyController.OnCollide.Invoke(direction * velocity * pushForce);
        }
    }
}
