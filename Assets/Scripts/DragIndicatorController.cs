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
    public LineRenderer LineRenderer { get; private set; }

    public bool IsDragging => LineRenderer.enabled;

    private void Awake()
    {
        LineRenderer.enabled = false;
        LineRenderer.positionCount = 2;
    }

    public void StartDrag(Vector3 start)
    {
        LineRenderer.enabled = true;
        LineRenderer.startColor = StartColor;
        LineRenderer.endColor = EndColor;

        LineRenderer.SetPosition(0, start);
        LineRenderer.SetPosition(1, start);
    }

    public void Drag(Vector3 position)
        => LineRenderer.SetPosition(1, position);

    public void EndDrag()
    {
        LineRenderer.enabled = false;
        LineRenderer.SetPosition(0, Vector3.zero);
        LineRenderer.SetPosition(1, Vector3.zero);
    }

    public Vector2 GetLaunchVector()
    {
        var start = LineRenderer.GetPosition(0);
        var end = LineRenderer.GetPosition(1);

        var direction = (start - end).normalized;
        var distance = Vector2.Distance(end, start);

        return direction * (distance * ForceMultiplier);
    }
}
