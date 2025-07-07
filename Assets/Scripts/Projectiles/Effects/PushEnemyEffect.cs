using UnityEngine;

public class PushEnemyEffect : ProjectileEffect
{
    [field: SerializeField]
    public float ForceMultiplier { get; private set; } = 5f;

    [field: SerializeField]
    public float MaxPushForce { get; private set; } = 10f;

    public override void OnEnemyCollide(ProjectileController projectile, EnemyController target, Collision2D collision)
    {
        var direction = (target.transform.position - projectile.transform.position).normalized;
        var velocity = collision.relativeVelocity.magnitude;
        var pushForce = Mathf.Min(velocity * ForceMultiplier, MaxPushForce);

        target.OnCollide.Invoke(direction * pushForce);
    }
}
