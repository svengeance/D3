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
    private float InternalDriveSpeed { get; set; } = 1f;

    [field: SerializeField]
    private float InternalAcceleration { get; set; } = 12f;

    [field: SerializeField]
    private float InternalTurnSpeed { get; set; } = 80f;

    [field: SerializeField]
    private float MinTurnDriveMultiplier { get; set; } = 0.8f;

    [field: SerializeField]
    private float ExternalForceDecayRate { get; set; } = 3f;

    [field: SerializeField]
    private float ExternalSpinDecayRate { get; set; } = 1.2f;

    [field: SerializeField]
    private float MaxExternalSpinSpeed { get; set; } = 1440f;

    private PlayerController Player { get; set; }

    private Vector2 LastMovementDirection { get; set; } = Vector2.left;

    private float TurnSpeedPenaltyMultiplier { get; set; } = 1f;

    private float VerticalBufferAroundPlayer { get; set; }

    private Vector2 ExternalVelocity { get; set; } = Vector2.zero;

    private float ExternalAngularVelocity { get; set; }

    private float InternalDriveVelocity { get; set; }

    private void Awake()
    {
        VerticalBufferAroundPlayer = Random.Range(0.3f, 1f);
        LastMovementDirection = -transform.right;
    }

    private void Start()
    {
        Player = PlayerManager.Instance.Player;
        InternalDriveVelocity = InternalDriveSpeed;
    }

    private void FixedUpdate()
    {
        var steeringDirection = CalculateSteering();
        var externalVelocity = CalculateExternalVelocity();
        var bodyRotation = RotateTowardSteering(steeringDirection);
        var facingRadians = (bodyRotation - 180f) * Mathf.Deg2Rad;
        var facingDirection = new Vector2(Mathf.Cos(facingRadians), Mathf.Sin(facingRadians));

        RigidBody.rotation = bodyRotation;
        var targetDriveVelocity = InternalDriveSpeed * TurnSpeedPenaltyMultiplier;
        InternalDriveVelocity = Mathf.MoveTowards(
            InternalDriveVelocity,
            targetDriveVelocity,
            InternalAcceleration * Time.fixedDeltaTime);

        RigidBody.linearVelocity = facingDirection * InternalDriveVelocity + externalVelocity;
        RigidBody.angularVelocity = 0f;
    }

    public void ApplyExternalVelocity(Vector2 velocityDelta)
        => ExternalVelocity += velocityDelta;

    public void ApplyExternalSpin(float angularVelocityDelta)
        => ExternalAngularVelocity = Mathf.Clamp(
            ExternalAngularVelocity + angularVelocityDelta,
            -MaxExternalSpinSpeed,
            MaxExternalSpinSpeed);

    private Vector2 CalculateSteering()
    {
        var moveLeft = Vector2.left;
        var playerAvoidance = CalculateMovementAroundPlayer();
        var enemyAvoidance = CalculateEnemyMovementAroundEachOther();

        return (moveLeft + playerAvoidance + enemyAvoidance).normalized;
    }

    private Vector2 CalculateMovementAroundPlayer()
    {
        if (Player is null)
            return Vector2.zero;

        var playerBounds = Player.Collider.bounds;
        var horizontalStartBuffer = 1.5f;

        var distanceFromPlayerX = Mathf.Max(0f, RigidBody.position.x - playerBounds.max.x);
        if (distanceFromPlayerX > horizontalStartBuffer)
            return Vector2.zero;

        var verticalDistanceToEdge = Mathf.Max(0f, Mathf.Abs(RigidBody.position.y - playerBounds.center.y) - playerBounds.extents.y) / VerticalBufferAroundPlayer;
        var verticalFalloff = 1f - Mathf.Clamp01(verticalDistanceToEdge);
        verticalFalloff = Mathf.SmoothStep(0f, 1f, verticalFalloff);

        var horizontalStrength = 1f - Mathf.Clamp01(distanceFromPlayerX / horizontalStartBuffer);
        horizontalStrength = Mathf.SmoothStep(0f, 1f, horizontalStrength);

        var finalFalloff = verticalFalloff * horizontalStrength;
        if (finalFalloff <= 0.001f)
            return Vector2.zero;

        var yOffset = RigidBody.position.y - playerBounds.center.y;
        var verticalAvoidance = Vector2.up * Mathf.Sign(yOffset);
        return verticalAvoidance * (6.0f * finalFalloff);
    }

    private Vector2 CalculateEnemyMovementAroundEachOther()
    {
        var separationRadius = 1.0f;
        var neighbors = new List<Collider2D>();
        Physics2D.OverlapCircle(
            RigidBody.position,
            separationRadius,
            new ContactFilter2D { useLayerMask = true, layerMask = EnemyLayerMask },
            neighbors);

        var separationForce = Vector2.zero;

        foreach (var neighbor in neighbors)
        {
            if (neighbor.attachedRigidbody == RigidBody)
                continue;

            var away = (RigidBody.position - (Vector2)neighbor.transform.position).normalized;
            var distance = Vector2.Distance(RigidBody.position, neighbor.transform.position);
            var strength = Mathf.Clamp01(1f - distance / separationRadius);

            separationForce += away * strength;
        }

        return separationForce;
    }

    private Vector2 CalculateExternalVelocity()
    {
        var result = ExternalVelocity;
        ExternalVelocity = Vector2.Lerp(ExternalVelocity, Vector2.zero, Time.fixedDeltaTime * ExternalForceDecayRate);
        return result;
    }

    private float RotateTowardSteering(Vector2 steeringDirection)
    {
        if (Mathf.Approximately(steeringDirection.sqrMagnitude, 0f))
            return RigidBody.rotation;

        var targetMovementAngle = Mathf.Atan2(steeringDirection.y, steeringDirection.x) * Mathf.Rad2Deg;
        var targetBodyAngle = targetMovementAngle + 180f;
        var nextBodyAngle = Mathf.MoveTowardsAngle(
            RigidBody.rotation,
            targetBodyAngle,
            InternalTurnSpeed * Time.fixedDeltaTime);

        nextBodyAngle += ExternalAngularVelocity * Time.fixedDeltaTime;
        ExternalAngularVelocity = Mathf.Lerp(ExternalAngularVelocity, 0f, Time.fixedDeltaTime * ExternalSpinDecayRate);

        LastMovementDirection = steeringDirection;
        TurnSpeedPenaltyMultiplier = CalculateTurnSpeedMultiplier(nextBodyAngle, targetBodyAngle);

        return nextBodyAngle;
    }

    private float CalculateTurnSpeedMultiplier(float bodyAngle, float targetBodyAngle)
    {
        var deltaTurnAngle = Mathf.Abs(Mathf.DeltaAngle(bodyAngle, targetBodyAngle)) / 90f;
        var curveMultiplier = 2f;
        var exaggeratedTurn = Mathf.SmoothStep(0f, 1f, deltaTurnAngle * curveMultiplier);
        var targetSpeedMultiplier = Mathf.Lerp(1f, MinTurnDriveMultiplier, exaggeratedTurn);
        var inertiaSpeed = 8f;

        return Mathf.Lerp(TurnSpeedPenaltyMultiplier, targetSpeedMultiplier, Time.fixedDeltaTime * inertiaSpeed);
    }
}
