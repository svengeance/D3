using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovementBehavior : MonoBehaviour
{
    [SerializeField]
    private EnemyController _enemy;

    private float _currentSpeedMultiplier = 1f;

    private Vector2 _lastMovementDirection = Vector2.left;

    private Rigidbody2D _rb;

    private float _verticalBufferAroundPlayer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _verticalBufferAroundPlayer = Random.Range(0.3f, 1f);
    }

    private void FixedUpdate()
    {
        var movement = CalculateMovement();

        FacePlayerToMovement(movement);

        _rb.linearVelocity = movement;
    }

    private Vector2 CalculateMovement()
    {
        var moveLeft = Vector2.left;
        var playerAvoidance = CalculateMovementAroundPlayer();
        var enemyAvoidance = CalculateEnemyMovementAroundEachOther();

        var desiredMovement = (moveLeft + playerAvoidance + enemyAvoidance * 1).normalized;

        // How much did the movement direction change compared to last frame?
        var deltaTurnAngle = Vector2.Angle(_lastMovementDirection, desiredMovement) / 90f;

        // Amplify mid-to-low turns stronger
        var curveMultiplier = 20f;
        var exaggeratedTurn = Mathf.SmoothStep(0f, 1f, deltaTurnAngle * curveMultiplier);
        var targetSpeedMultiplier = Mathf.Lerp(1f, 0.10f, exaggeratedTurn);
        var inertiaSpeed = 8f;

        _currentSpeedMultiplier = Mathf.Lerp(_currentSpeedMultiplier, targetSpeedMultiplier, Time.fixedDeltaTime * inertiaSpeed);
        _lastMovementDirection = desiredMovement; // Update for next frame

        return desiredMovement * _currentSpeedMultiplier;
    }

    private Vector2 CalculateMovementAroundPlayer()
    {
        var playerBounds = _enemy.Player.Collider.bounds;
        var horizontalStartBuffer = 1.5f; // Horizontal range to start avoidance

        var distanceFromPlayerX = Mathf.Max(0f, _rb.position.x - playerBounds.max.x);
        if (distanceFromPlayerX > horizontalStartBuffer)
            return Vector2.zero; // Too far right, no vertical movement yet

        // Vertical avoidance strength
        var verticalDistanceToEdge = Mathf.Max(0f, Mathf.Abs(_rb.position.y - playerBounds.center.y) - playerBounds.extents.y) / _verticalBufferAroundPlayer;
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
        var yOffset = _rb.position.y - playerBounds.center.y;
        var verticalAvoidance = Vector2.up * Mathf.Sign(yOffset);

        return verticalAvoidance * (6.0f * finalFalloff);
    }

    private Vector2 CalculateEnemyMovementAroundEachOther()
    {
        var separationRadius = 1.0f; // Radius to check for other enemies
        var neighbors = new List<Collider2D>();
        Physics2D.OverlapCircle(_rb.position, separationRadius, new ContactFilter2D(), neighbors);

        var separationForce = Vector2.zero;

        foreach (var neighbor in neighbors)
        {
            if (neighbor.attachedRigidbody == _rb)
                continue; // skip self

            var away = (_rb.position - (Vector2)neighbor.transform.position).normalized;
            var distance = Vector2.Distance(_rb.position, neighbor.transform.position);
            var strength = Mathf.Clamp01(1f - distance / separationRadius);

            separationForce += away * strength;
        }

        return separationForce;
    }

    private void FacePlayerToMovement(Vector2 movement)
    {
        if (Mathf.Approximately(movement.sqrMagnitude, 0f))
            return;

        _rb.rotation = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg + 180f;
    }
}
