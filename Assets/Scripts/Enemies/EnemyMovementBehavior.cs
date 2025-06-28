using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovementBehavior : MonoBehaviour
{
    [field: SerializeField]
    private EnemyController Enemy { get; set; }

    [field: SerializeField]
    private Rigidbody2D RigidBody { get; set; }

    [field: SerializeField]
    private LayerMask EnemyLayerMask { get; set; }

    private PlayerController Player { get; set; }

    private Vector2 LastMovementDirection { get; set; } = Vector2.left;

    private float CurrentSpeedMultiplier { get; set; } = 1f;

    private float VerticalBufferAroundPlayer { get; set; }

    private void Awake()
    {
        VerticalBufferAroundPlayer = Random.Range(0.3f, 1f);
        Player = PlayerManager.Instance.Player;
    }

    private void FixedUpdate()
    {
        var movement = CalculateMovement();

        FacePlayerToMovement(movement);

        RigidBody.linearVelocity = movement;
    }

    private Vector2 CalculateMovement()
    {
        var moveLeft = Vector2.left;
        var playerAvoidance = CalculateMovementAroundPlayer();
        var enemyAvoidance = CalculateEnemyMovementAroundEachOther();

        var desiredMovement = (moveLeft + playerAvoidance + enemyAvoidance * 1).normalized;

        // How much did the movement direction change compared to last frame?
        var deltaTurnAngle = Vector2.Angle(LastMovementDirection, desiredMovement) / 90f;

        // Amplify mid-to-low turns stronger
        var curveMultiplier = 20f;
        var exaggeratedTurn = Mathf.SmoothStep(0f, 1f, deltaTurnAngle * curveMultiplier);
        var targetSpeedMultiplier = Mathf.Lerp(1f, 0.10f, exaggeratedTurn);
        var inertiaSpeed = 8f;

        CurrentSpeedMultiplier = Mathf.Lerp(CurrentSpeedMultiplier, targetSpeedMultiplier, Time.fixedDeltaTime * inertiaSpeed);
        LastMovementDirection = desiredMovement; // Update for next frame

        return desiredMovement * CurrentSpeedMultiplier;
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

    private void FacePlayerToMovement(Vector2 movement)
    {
        if (Mathf.Approximately(movement.sqrMagnitude, 0f))
            return;

        RigidBody.rotation = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg + 180f;
    }
}
