using UnityEngine;

public class ExplodeOnContactBehavior : ProjectileBehavior
{
    [field: SerializeField]
    public float ForceMultiplier { get; private set; } = 1f;

    [field: SerializeField]
    public float MaxPushForce { get; private set; } = 5f;
}
