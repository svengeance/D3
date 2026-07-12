using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovementBehavior : MonoBehaviour
{
    [field: SerializeField]
    public Rigidbody2D RigidBody { get; private set; }

    [field: SerializeField]
    private float InternalDriveSpeed { get; set; } = 1f;

    [field: SerializeField]
    private float InternalAcceleration { get; set; } = 12f;

    [field: SerializeField]
    private float InternalTurnSpeed { get; set; } = 80f;

    [field: SerializeField]
    private float MinTurnDriveMultiplier { get; set; } = 0.8f;

    private PlayerController Player { get; set; }

    private float TurnSpeedPenaltyMultiplier { get; set; } = 1f;

    private float VerticalBufferAroundPlayer { get; set; }

    private float InternalDriveVelocity { get; set; }

    private void Awake()
        => VerticalBufferAroundPlayer = Random.Range(0.3f, 1f);

    private void Start()
    {
        Player = PlayerManager.Instance.Player;
        InternalDriveVelocity = InternalDriveSpeed;
    }

    private void FixedUpdate()
    {
        var steeringDirection = CalculateSteering();
        var bodyRotation = RotateTowardSteering(steeringDirection);
        var facingRadians = (bodyRotation - 180f) * Mathf.Deg2Rad;
        var facingDirection = new Vector2(Mathf.Cos(facingRadians), Mathf.Sin(facingRadians));

        var targetDriveVelocity = InternalDriveSpeed * TurnSpeedPenaltyMultiplier;
        InternalDriveVelocity = Mathf.MoveTowards(InternalDriveVelocity, targetDriveVelocity, InternalAcceleration * Time.fixedDeltaTime);

        var targetVelocity = facingDirection * InternalDriveVelocity;
        var velocityChange = Vector2.ClampMagnitude(targetVelocity - RigidBody.linearVelocity, InternalAcceleration * Time.fixedDeltaTime);

        RigidBody.AddForce(velocityChange * RigidBody.mass, ForceMode2D.Impulse);
    }

    private Vector2 CalculateSteering()
    {
        var moveLeft = Vector2.left;
        var playerAvoidance = CalculateMovementAroundPlayer();

        return (moveLeft + playerAvoidance).normalized;
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

    private float RotateTowardSteering(Vector2 steeringDirection)
    {
        if (Mathf.Approximately(steeringDirection.sqrMagnitude, 0f))
            return RigidBody.rotation;

        var targetMovementAngle = Mathf.Atan2(steeringDirection.y, steeringDirection.x) * Mathf.Rad2Deg;
        var targetBodyAngle = targetMovementAngle + 180f;
        var turnDelta = Mathf.DeltaAngle(RigidBody.rotation, targetBodyAngle);
        var rotationThisStep = RigidBody.angularVelocity * Time.fixedDeltaTime;

        if (Mathf.Sign(turnDelta) == Mathf.Sign(rotationThisStep) && Mathf.Abs(turnDelta) <= Mathf.Abs(rotationThisStep))
        {
            RigidBody.MoveRotation(targetBodyAngle);
            RigidBody.angularVelocity = 0f;
            TurnSpeedPenaltyMultiplier = 1f;
            return targetBodyAngle;
        }

        var desiredAngularVelocity = Mathf.Clamp(turnDelta * 4f, -InternalTurnSpeed, InternalTurnSpeed);
        var angularVelocityChange = desiredAngularVelocity - RigidBody.angularVelocity;
        RigidBody.AddTorque(angularVelocityChange * Mathf.Deg2Rad * RigidBody.inertia, ForceMode2D.Force);

        TurnSpeedPenaltyMultiplier = CalculateTurnSpeedMultiplier(RigidBody.rotation, targetBodyAngle);

        return RigidBody.rotation;
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
