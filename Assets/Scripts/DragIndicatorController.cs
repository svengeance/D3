using UnityEngine;

public class DragIndicatorController : MonoBehaviour
{
    [field: SerializeField]
    public float MaxDragDistance { get; private set; } = 2f;

    [field: SerializeField]
    public float MinDragDistance { get; private set; } = 0.5f;

    [field: SerializeField]
    public float ForceMultiplier { get; private set; } = 5f;

    [field: SerializeField]
    public Color StartColor { get; private set; }

    [field: SerializeField]
    public Color EndColor { get; private set; }

    [field: SerializeField]
    public LineRenderer LaunchRenderer { get; private set; }

    [field: SerializeField]
    public LineRenderer GuidelineRenderer { get; private set; }

    [field: SerializeField]
    public bool EnableGuideline { get; private set; } = true;

    public bool IsDragging => LaunchRenderer.enabled;

    private void Awake()
    {
        LaunchRenderer.enabled = false;
        LaunchRenderer.positionCount = 2;
    }

    public void StartDrag(Vector3 start)
    {
        LaunchRenderer.enabled = true;
        LaunchRenderer.startColor = StartColor;
        LaunchRenderer.endColor = EndColor;

        LaunchRenderer.SetPosition(0, start);
        LaunchRenderer.SetPosition(1, start);
    }

    public void Drag(Vector3 position)
    {
        LaunchRenderer.SetPosition(1, position);

        SyncGuideline();
    }

    public void EndDrag()
    {
        LaunchRenderer.enabled = false;
        LaunchRenderer.SetPosition(0, Vector3.zero);
        LaunchRenderer.SetPosition(1, Vector3.zero);

        SyncGuideline();
    }

    public Vector2 GetLaunchVector()
    {
        var start = LaunchRenderer.GetPosition(0);
        var end = LaunchRenderer.GetPosition(1);

        var direction = (start - end).normalized;
        var distance = Vector2.Distance(end, start);

        return direction * (distance * ForceMultiplier);
    }

    private void SyncGuideline()
    {
        GuidelineRenderer.enabled = LaunchRenderer.enabled;

        if (!EnableGuideline)
            return;

        var launchVector = GetLaunchVector();
        var guidelineStart = LaunchRenderer.GetPosition(0);
        var guidelineEnd = launchVector.normalized * 100f;

        GuidelineRenderer.positionCount = 2;
        GuidelineRenderer.SetPosition(0, guidelineStart);
        GuidelineRenderer.SetPosition(1, guidelineEnd);
    }
}
