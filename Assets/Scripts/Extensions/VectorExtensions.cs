using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 ClosestPointInCircle(this Vector2 point, Vector2 circleCenter, float radius)
    {
        var direction = point - circleCenter;
        var distance = direction.magnitude;

        if (distance <= radius)
            return point; // Already inside the circle

        return circleCenter + direction.normalized * radius;
    }
}
