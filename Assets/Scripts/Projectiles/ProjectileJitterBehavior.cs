using UnityEngine;

public class ProjectileJitterBehavior : MonoBehaviour
{
    [field: SerializeField]
    public float ScaleAmount { get; private set; } = 0.05f;

    [field: SerializeField]
    public float ScaleFrequency { get; private set; } = 5f;

    [field: SerializeField]
    public float JitterAmount { get; private set; } = 0.075f;

    [field: SerializeField]
    public float JitterSpeed { get; private set; } = 10f;

    private Vector3 OriginalPosition { get; set; }

    private Vector3 OriginalScale { get; set; }

    private void Start()
    {
        OriginalScale = transform.localScale;
        OriginalPosition = transform.localPosition;
    }

    private void Update()
    {
        // Pulse scale
        var pulse = Mathf.Sin(Time.time * ScaleFrequency) * ScaleAmount;
        transform.localScale = OriginalScale * (1f + pulse);

        // Tiny positional jitter
        var jitterX = Mathf.PerlinNoise(Time.time * JitterSpeed, 0f) - 0.5f;
        var jitterY = Mathf.PerlinNoise(0f, Time.time * JitterSpeed) - 0.5f;
        var jitter = new Vector3(jitterX, jitterY, 0f) * JitterAmount;

        transform.localPosition = OriginalPosition + jitter;
    }
}
