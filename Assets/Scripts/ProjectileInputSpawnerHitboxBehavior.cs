using UnityEngine;

public class ProjectileInputSpawnerHitboxBehavior : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public ProjectileInputSpawnerController Controller { get; private set; }

    [field: SerializeField]
    public LineRenderer BorderIndicator { get; private set; }

    [field: SerializeField]
    public CircleCollider2D Collider { get; private set; }

    [field: SerializeField]
    public int BorderIndicatorSegments { get; private set; }

    [field: SerializeField]
    public float RotationSpeed { get; private set; }

    private bool _borderIndicatorRotating;

    private DragIndicatorController _dragIndicator;

    private float _initialColliderRadius;

    private void Awake()
    {
        _dragIndicator = Controller.DragIndicator;
        _initialColliderRadius = Collider.radius;

        InitializeBorderIndicator();
    }

    private void Update()
    {
        if (!_borderIndicatorRotating)
            return;

        var offset = Time.time * RotationSpeed;
        BorderIndicator.material.mainTextureOffset = new Vector2(offset, 0);
    }

    public void OnDragStart(Vector2 pos)
    {
        _dragIndicator.StartDrag(pos);

        Collider.radius = 100f;
    }

    public void OnDragEnd(Vector2 pos)
    {
        if (!Collider.bounds.Contains(pos))
            DisableSpawner();

        _dragIndicator.EndDrag();

        Collider.radius = _initialColliderRadius;
    }

    public void OnDrag(Vector2 pos)
        => _dragIndicator.Drag(pos);

    public void OnPointerEnter(Vector2 pos)
        => EnableSpawner();

    public void OnPointerExit(Vector2 pos)
    {
        if (!_dragIndicator.IsDragging)
            DisableSpawner();
    }

    private void EnableSpawner()
    {
        _borderIndicatorRotating = true;

        BorderIndicator.material.color = Color.white;
    }

    private void DisableSpawner()
    {
        _borderIndicatorRotating = false;
        _dragIndicator.EndDrag();

        BorderIndicator.material.color = Color.gray;
    }

    private void InitializeBorderIndicator()
    {
        BorderIndicator.material.color = Color.gray;
        BorderIndicator.positionCount = BorderIndicatorSegments;

        for (var i = 0; i < BorderIndicatorSegments; i++)
        {
            var angle = i * Mathf.PI * 2f / BorderIndicatorSegments;
            var x = Mathf.Cos(angle) * Collider.radius;
            var y = Mathf.Sin(angle) * Collider.radius;
            BorderIndicator.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}
