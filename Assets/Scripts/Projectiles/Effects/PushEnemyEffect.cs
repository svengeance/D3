using UnityEngine;

public class PushEnemyEffect : ProjectileBehavior
{
    [field: SerializeField]
    public float ForceMultiplier { get; private set; } = 5f;

    [field: SerializeField]
    public float MaxPushForce { get; private set; } = 10f;

    public override void OnEnemyCollide(ProjectileController projectile, EnemyController target, Vector2 relativeVelocity)
    {
        var direction = (target.transform.position - projectile.transform.position).normalized;
        var velocity = relativeVelocity.magnitude;
        var pushForce = Mathf.Min(velocity * ForceMultiplier, MaxPushForce);

        target.OnCollide.Invoke(direction * pushForce);
    }
}
