using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovementBehavior : MonoBehaviour
{
    [field: SerializeField]
    public Rigidbody2D RigidBody { get; private set; }

    [field: SerializeField]
    private LayerMask EnemyLayerMask { get; set; }

    private PlayerController Player { get; set; }

    private Vector2 LastMovementDirection { get; set; } = Vector2.left;

    private float TurnSpeedPenaltyMultiplier { get; set; } = 1f;

    private float VerticalBufferAroundPlayer { get; set; }

    private Vector2 ExternalForce { get; set; } = Vector2.zero;

    private float ExternalForceDecayRate { get; } = 2.5f;

    private float SpinOffset { get; set; }

    private float SpinVelocity { get; set; }

    private float SpinSmoothTime { get; } = 0.35f; // damped ease so knockback spin arcs back into place instead of snapping

    private void Awake()
        => VerticalBufferAroundPlayer = Random.Range(0.3f, 1f);

    private void Start()
        => Player = PlayerManager.Instance.Player;

    private void FixedUpdate()
    {
        var steeringDirection = CalculateSteering();
        var externalForce = CalculateExternalForce();

        // Drive velocity is locked to the tank's actual facing (steering + any unresolved spin),
        // so a knockback that spins the nose off-course makes it drive an arc back on course
        // instead of sliding there in a straight line. externalForce is added on top, unlocked
        // from facing, so a side impact still shoves/slides the tank regardless of where it's pointed.
        var facingDirection = FaceTowardSteering(steeringDirection);

        RigidBody.linearVelocity = facingDirection * TurnSpeedPenaltyMultiplier + externalForce;
        RigidBody.angularVelocity = 0f; // script owns rotation directly; clear real collision torque so it can't leak into interpolation
    }

    public void OnCollide(Vector2 force)
        => ExternalForce += force;

    public void ApplySpin(float degrees)
        => SpinOffset += degrees;

    private Vector2 CalculateSteering()
    {
        var moveLeft = Vector2.left;
        var playerAvoidance = CalculateMovementAroundPlayer();
        var enemyAvoidance = CalculateEnemyMovementAroundEachOther();

        LastMovementDirection = (moveLeft + playerAvoidance + enemyAvoidance * 1).normalized;
        TurnSpeedPenaltyMultiplier = CalculateTurnSpeedMultiplier(LastMovementDirection);

        return LastMovementDirection;
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

        return separationForce;
    }

    private Vector2 CalculateExternalForce()
    {
        var result = ExternalForce; // Use full force this frame

        ExternalForce = Vector2.Lerp(ExternalForce, Vector2.zero, Time.fixedDeltaTime * ExternalForceDecayRate);

        return result;
    }

    private float CalculateTurnSpeedMultiplier(Vector2 movement)
    {
        // How much did the movement direction change compared to last frame?
        var deltaTurnAngle = Vector2.Angle(LastMovementDirection, movement) / 90f;
        var curveMultiplier = 20f;
        var exaggeratedTurn = Mathf.SmoothStep(0f, 1f, deltaTurnAngle * curveMultiplier);
        var targetSpeedMultiplier = Mathf.Lerp(1f, 0.10f, exaggeratedTurn);
        var inertiaSpeed = 8f;

        return Mathf.Lerp(TurnSpeedPenaltyMultiplier, targetSpeedMultiplier, Time.fixedDeltaTime * inertiaSpeed);
    }

    private Vector2 FaceTowardSteering(Vector2 steeringDirection)
    {
        if (Mathf.Approximately(steeringDirection.sqrMagnitude, 0f))
            return steeringDirection;

        var targetAngle = Mathf.Atan2(steeringDirection.y, steeringDirection.x) * Mathf.Rad2Deg;
        var facingAngle = targetAngle + SpinOffset;

        RigidBody.rotation = facingAngle + 180f;

        var spinVelocity = SpinVelocity;
        SpinOffset = Mathf.SmoothDamp(SpinOffset, 0f, ref spinVelocity, SpinSmoothTime);
        SpinVelocity = spinVelocity;

        var facingRadians = facingAngle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(facingRadians), Mathf.Sin(facingRadians));
    }
}
