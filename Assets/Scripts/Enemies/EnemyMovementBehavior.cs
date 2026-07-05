using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovementBehavior : MonoBehaviour
{
    [field: SerializeField]
    public Rigidbody2D RigidBody { get; private set; }

    [field: SerializeField]
    private LayerMask EnemyLayerMask { get; set; }

    [field: SerializeField]
    private Transform Goalpost { get; set; }

    [field: SerializeField]
    private float MoveSpeed { get; set; } = 0.95f;

    // How hard the enemy accelerates toward its target cruising velocity. This
    // doubles as the "slowing force": when the enemy is knocked, it opposes the
    // excess velocity and eases it back onto its path. Lower = weightier knockback
    // that lingers longer; higher = snappier recovery.
    [field: SerializeField]
    private float SteeringResponsiveness { get; set; } = 3f;

    [field: SerializeField]
    private float MaxLateralTractionImpulse { get; set; } = 0.22f;

    // Facing uses a capped turn motor rather than a raw spring. The target heading
    // asks for an angular velocity, but the tank can only apply limited torque to
    // get there. That avoids underdamped see-saw correction while preserving impact
    // spin because projectile angular velocity cannot be cancelled in one frame.
    [field: SerializeField]
    private float TurnResponsiveness { get; set; } = 1.9f;

    [field: SerializeField]
    private float TurnDamping { get; set; } = 0.032f;

    [field: SerializeField]
    private float MaxTurnSpeed { get; set; } = 24f;

    [field: SerializeField]
    private float MaxTurnTorque { get; set; } = 1.15f;

    [field: SerializeField]
    private float FacingDeadZone { get; set; } = 3f;

    private PlayerController Player { get; set; }

    private Vector2 LastMovementDirection { get; set; } = Vector2.left;

    private float VerticalBufferAroundPlayer { get; set; }

    private void Awake()
        => VerticalBufferAroundPlayer = Random.Range(0.3f, 1f);

    private void Start()
    {
        Player = PlayerManager.Instance.Player;

        if (Goalpost == null)
        {
            var goalpost = GameObject.Find("EnemyGoalpost");
            if (goalpost != null)
                Goalpost = goalpost.transform;
        }
    }

    private void FixedUpdate()
    {
        var desiredDirection = CalculateDesiredDirection();

        ApplySteering(desiredDirection);
        ApplyFacing(desiredDirection);
    }

    private Vector2 CalculateDesiredDirection()
    {
        var goalSeeking = CalculateGoalSeekingMovement();
        var playerAvoidance = CalculateMovementAroundPlayer();
        var enemyAvoidance = CalculateEnemyMovementAroundEachOther();

        var desiredDirection = (goalSeeking + playerAvoidance + enemyAvoidance).normalized;
        if (desiredDirection.sqrMagnitude <= 0.0001f)
            desiredDirection = LastMovementDirection;

        LastMovementDirection = Vector2.Lerp(LastMovementDirection, desiredDirection, Time.fixedDeltaTime * 7.5f).normalized;

        return LastMovementDirection;
    }

    private void ApplySteering(Vector2 desiredDirection)
    {
        // Drive along the direction the enemy is FACING (vehicle-like), not
        // straight at the goal. Combined with a slow turn, this forces the enemy
        // to physically arc to change heading instead of sliding goal-ward while
        // pivoting in place. Throttle only acts forward/back through the nose.
        // Sideways slip is handled separately as capped traction so normal driving
        // feels grippy, while hard projectile hits can still skid and recover.
        var facingDirection = FacingDirection();
        var forwardSpeed = Vector2.Dot(RigidBody.linearVelocity, facingDirection);
        var throttleForce = facingDirection * ((MoveSpeed - forwardSpeed) * SteeringResponsiveness * RigidBody.mass);

        RigidBody.AddForce(throttleForce);
        ApplyLateralTraction(facingDirection);
    }

    private void ApplyLateralTraction(Vector2 facingDirection)
    {
        var lateralDirection = Vector2.Perpendicular(facingDirection).normalized;
        var lateralVelocity = lateralDirection * Vector2.Dot(RigidBody.linearVelocity, lateralDirection);
        var desiredImpulse = -lateralVelocity * RigidBody.mass;
        var tractionImpulse = Vector2.ClampMagnitude(desiredImpulse, MaxLateralTractionImpulse);

        RigidBody.AddForce(tractionImpulse, ForceMode2D.Impulse);
    }

    // Unit vector the enemy is currently pointing (drives) along, derived from its
    // rotation. Inverse of the facing convention used in ApplyFacing.
    private Vector2 FacingDirection()
    {
        var rad = (RigidBody.rotation - 180f) * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    private void ApplyFacing(Vector2 desiredDirection)
    {
        if (desiredDirection.sqrMagnitude < 0.0001f)
            return;

        var targetAngle = Mathf.Atan2(desiredDirection.y, desiredDirection.x) * Mathf.Rad2Deg + 180f;
        var error = Mathf.DeltaAngle(RigidBody.rotation, targetAngle);
        if (Mathf.Abs(error) < FacingDeadZone)
            error = 0f;

        var targetAngularVelocity = Mathf.Clamp(error * TurnResponsiveness, -MaxTurnSpeed, MaxTurnSpeed);
        var turnTorque = (targetAngularVelocity - RigidBody.angularVelocity) * TurnDamping;
        turnTorque = Mathf.Clamp(turnTorque, -MaxTurnTorque, MaxTurnTorque);

        RigidBody.AddTorque(turnTorque);
    }

    private Vector2 CalculateGoalSeekingMovement()
    {
        if (Goalpost)
        {
            var toGoal = (Vector2)Goalpost.position - RigidBody.position;
            if (toGoal.sqrMagnitude > 0.01f)
                return toGoal.normalized;
        }

        return Vector2.left;
    }

    private Vector2 CalculateMovementAroundPlayer()
    {
        if (Player is null)
            return Vector2.zero; // No player reference, no avoidance

        var playerBounds = Player.Collider.bounds;
        var horizontalStartBuffer = 1.5f; // Horizontal range to start avoidance

        var distanceFromPlayerX = Mathf.Max(0f, RigidBody.position.x - playerBounds.max.x);
        if (distanceFromPlayerX > horizontalStartBuffer)
            return Vector2.zero; // Too far right, no vertical movement yet

        // Vertical avoidance strength
        var verticalDistanceToEdge = Mathf.Max(0f, Mathf.Abs(RigidBody.position.y - playerBounds.center.y) - playerBounds.extents.y) / VerticalBufferAroundPlayer;
        var verticalFalloff = 1f - Mathf.Clamp01(verticalDistanceToEdge);
        verticalFalloff = Mathf.SmoothStep(0f, 1f, verticalFalloff);

        // Horizontal proximity strength
        var horizontalStrength = 1f - Mathf.Clamp01(distanceFromPlayerX / horizontalStartBuffer);
        horizontalStrength = Mathf.SmoothStep(0f, 1f, horizontalStrength);

        // Final vertical avoidance strength
        var finalFalloff = verticalFalloff * horizontalStrength;

        if (finalFalloff <= 0.001f)
            return Vector2.zero;

        // Choose avoidance direction
        var yOffset = RigidBody.position.y - playerBounds.center.y;
        var verticalAvoidance = Vector2.up * Mathf.Sign(yOffset);

        return verticalAvoidance * (6.0f * finalFalloff);
    }

    private Vector2 CalculateEnemyMovementAroundEachOther()
    {
        var separationRadius = 1.0f; // Radius to check for other enemies
        var neighbors = new List<Collider2D>();
        Physics2D.OverlapCircle(RigidBody.position, separationRadius, new ContactFilter2D { useLayerMask = true, layerMask = EnemyLayerMask }, neighbors);

        var separationForce = Vector2.zero;

        foreach (var neighbor in neighbors)
        {
            if (neighbor.attachedRigidbody == RigidBody)
                continue; // skip self

            var away = (RigidBody.position - (Vector2)neighbor.transform.position).normalized;
            var distance = Vector2.Distance(RigidBody.position, neighbor.transform.position);
            var strength = Mathf.Clamp01(1f - distance / separationRadius);

            separationForce += away * strength;
        }

        return separationForce * 1.25f;
    }
}
