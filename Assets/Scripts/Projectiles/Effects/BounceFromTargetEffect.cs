using UnityEngine;

public class BounceFromTargetEffect : ProjectileBehavior
{
    [field: SerializeField]
    public float BounceMultiplier { get; private set; } = 1.0f;

    [field: SerializeField]
    public float AngleVariance { get; private set; }
}
