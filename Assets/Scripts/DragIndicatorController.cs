using UnityEngine;

public class DragIndicatorController : MonoBehaviour
{
    [field: SerializeField]
    public float MaxDragDistance { get; private set; } = 2f;

    [field: SerializeField]
    public float MinDragDistance { get; private set; } = 0.5f;

    [field: SerializeField]
    public Color StartColor { get; private set; }

    [field: SerializeField]
    public Color EndColor { get; private set; }

    private LineRenderer _lineRenderer;

    public bool IsDragging => _lineRenderer.enabled;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
    }

    public void StartDrag(Vector3 start)
    {
        _lineRenderer.enabled = true;
        _lineRenderer.startColor = StartColor;
        _lineRenderer.endColor = EndColor;

        _lineRenderer.SetPosition(0, start);
        _lineRenderer.SetPosition(1, start);
    }

    public void Drag(Vector3 position)
        => _lineRenderer.SetPosition(1, position);

    public void EndDrag()
        => _lineRenderer.enabled = false;
}
